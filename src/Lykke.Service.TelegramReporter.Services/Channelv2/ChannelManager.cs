using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Dwh.Client;
using Lykke.Service.IndexHedgingEngine.Client;
using Lykke.Service.TelegramReporter.Core.Services;
using Lykke.Service.TelegramReporter.Core.Services.Channelv2;
using Lykke.Service.TelegramReporter.Services.Channelv2.Channels;
using Lykke.Service.TelegramReporter.Services.LiquidityEngine;

namespace Lykke.Service.TelegramReporter.Services.Channelv2
{
    public class ChannelManager : IChannelManager, IStartable, IStopable
    {
        private readonly List<ReportChannel> _channelList = new List<ReportChannel>();
        private readonly HashSet<string> _channelTypes = new HashSet<string>();

        private readonly IIndexHedgingEngineClient _indexHedgingEngineClient;
        private readonly IChannelRepository _channelRepository;
        private readonly ITelegramSender _telegramSender;
        private readonly ILogFactory _logFactory;
        private readonly LiquidityEngineUrlSettings _liquidityEngineUrlSettings;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IDwhClient _dwhClient;
        private readonly ILog _log;

        public ChannelManager(
            IIndexHedgingEngineClient indexHedgingEngineClient,
            IChannelRepository channelRepository,
            ITelegramSender telegramSender,
            ILogFactory logFactory,
            LiquidityEngineUrlSettings liquidityEngineUrlSettings,
            IAssetsServiceWithCache assetsServiceWithCache,
            IDwhClient dwhClient)
        {
            _indexHedgingEngineClient = indexHedgingEngineClient;
            _channelRepository = channelRepository;
            _telegramSender = telegramSender;
            _logFactory = logFactory;
            _liquidityEngineUrlSettings = liquidityEngineUrlSettings;
            _assetsServiceWithCache = assetsServiceWithCache;
            _dwhClient = dwhClient;
            _log = _logFactory.CreateLog(this);
            RegisterChannels();
        }

        private void RegisterChannels()
        {
            _channelTypes.Add(HelloWorldReportChannel.Name);
            _channelTypes.Add(LiquidityEngineSummaryChannel.Name);
            _channelTypes.Add(DwhStoreProcedureChannel.Name);
            _channelTypes.Add(LyciSandipOfferChannel.Name);
            _channelTypes.Add(IndexHedgingEngineHealthIssuesChannel.Name);
            _channelTypes.Add(LiquidityEngineMessagesChannel.Name);
        }

        private ReportChannel CreateReportChannel(IReportChannel channel)
        {
            try
            {
                if (channel.Type == HelloWorldReportChannel.Name)
                    return new HelloWorldReportChannel(channel, _telegramSender, _logFactory);

                if (channel.Type == LiquidityEngineSummaryChannel.Name)
                    return new LiquidityEngineSummaryChannel(channel, _telegramSender, _logFactory,
                        _liquidityEngineUrlSettings, _assetsServiceWithCache);

                if (channel.Type == DwhStoreProcedureChannel.Name)
                    return new DwhStoreProcedureChannel(channel, _telegramSender, _logFactory, _dwhClient);

                if (channel.Type == LyciSandipOfferChannel.Name)
                    return new LyciSandipOfferChannel(channel, _telegramSender, _logFactory, _dwhClient);

                if (channel.Type == IndexHedgingEngineHealthIssuesChannel.Name)
                    return new IndexHedgingEngineHealthIssuesChannel(channel, _telegramSender, _logFactory,
                        _indexHedgingEngineClient);

                if (channel.Type == LiquidityEngineMessagesChannel.Name)
                    return new LiquidityEngineMessagesChannel(channel, _telegramSender, _liquidityEngineUrlSettings, _logFactory);
            }
            catch (Exception ex)
            {
                _log.Warning($"Can't create channel '{channel.Type}'.", ex, channel.Type);
            }

            return null;
        }

        public Task<IReadOnlyList<string>> GetChanelTypesAsync()
        {
            return Task.FromResult(_channelTypes.ToList() as IReadOnlyList<string>);
        }

        public async Task<string> AddChannelAsync(string type, long chatId, TimeSpan interval, string metainfo)
        {
            var settings = new ReportChannelSettings(Guid.NewGuid().ToString(), type, chatId, interval, metainfo);
            var item = CreateReportChannel(settings);
            if (item == null)
            {
                _log.Error($"Incorrect channel Type = {settings.Type}.", context: $"Data: {settings.ToJson()}");
                throw new Exception($"Incorrect channel Type = {settings.Type}.");
            }

            await _channelRepository.AddOrUpdateChannelAsync(item);

            _channelList.Add(item);
            item.Start();

            return item.ChannelId;
        }

        public async Task<string> UpdateChannelAsync(string channelId, string type, long chatId, TimeSpan interval,
            string metainfo)
        {
            var settings = new ReportChannelSettings(Guid.NewGuid().ToString(), type, chatId, interval, metainfo);
            var item = CreateReportChannel(settings);
            if (item == null)
            {
                _log.Error($"Incorrect channel Type = {settings.Type}.", context: $"Data: {settings.ToJson()}");
                throw new Exception($"Incorrect channel Type = {settings.Type}.");
            }

            var old = _channelList.FirstOrDefault(e => e.ChannelId == channelId);
            if (old == null)
            {
                _log.Error($"Not found channel with Id = {channelId}.", context: $"Data: {settings.ToJson()}");
                throw new Exception($"Not found channel with Id = {channelId}.");
            }

            await _channelRepository.AddOrUpdateChannelAsync(item);
            _channelList.Remove(old);
            _channelList.Add(item);
            old.Stop();
            item.Start();
            _log.Info(
                $"Channel started, Id={item.ChannelId}, Type={item.GetType().FullName}, Metainfo: {item.Metainfo}");

            return item.ChannelId;
        }

        public async Task RemoveChannelAsync(string channelId)
        {
            var old = _channelList.FirstOrDefault(e => e.ChannelId == channelId);
            if (old == null)
                return;

            await _channelRepository.RemoveChannelAsync(channelId);
            _channelList.Remove(old);
            old.Stop();
        }

        public Task<IReadOnlyList<IReportChannel>> GetAllChannelsAsync()
        {
            return Task.FromResult<IReadOnlyList<IReportChannel>>(_channelList.Cast<IReportChannel>().ToList());
        }

        public void Start()
        {
            var channels = _channelRepository.GetAllChannelsAsync().Result;
            foreach (var channel in channels)
            {
                var item = CreateReportChannel(channel);
                if (item == null)
                {
                    _log.Error($"Incorrect channel Type = {channel.Type}.", context: $"Data: {channel.ToJson()}");
                    continue;
                }

                _channelList.Add(item);

                try
                {
                    item.Start();
                }
                catch (Exception ex)
                {
                    _channelList.Remove(item);

                    _log.Warning($"Can't start channel '{item.Type}'.", ex, item.Type);
                }


                _log.Info(
                    $"Channel started, Id={item.ChannelId}, Type={item.GetType().FullName}, Metainfo: {item.Metainfo}");
            }
        }

        public void Dispose()
        {
        }

        public void Stop()
        {
            foreach (var channel in _channelList)
            {
                channel.Stop();
            }
        }

        public class ReportChannelSettings : IReportChannel
        {
            public ReportChannelSettings(string channelId, string type, long chatId, TimeSpan interval, string metainfo)
            {
                ChannelId = channelId;
                Type = type;
                ChatId = chatId;
                Interval = interval;
                Metainfo = metainfo;
            }

            public string ChannelId { get; }
            public string Type { get; }
            public long ChatId { get; }
            public TimeSpan Interval { get; }
            public string Metainfo { get; }
        }
    }
}
