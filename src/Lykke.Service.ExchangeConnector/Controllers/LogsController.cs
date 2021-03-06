﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using TradingBot.Communications;

namespace TradingBot.Controllers
{
    public class LogsController : Controller
    {
        private readonly int entriesCount = 50;
        private readonly INoSQLTableStorage<LogEntity> _logsStorage;
        private readonly INoSQLTableStorage<FixMessageTableEntity> _fixMessagesStorage;
        private readonly TranslatedSignalsRepository _translatedSignalsRepository;

        public LogsController(
            INoSQLTableStorage<LogEntity> logsStorage,
            INoSQLTableStorage<FixMessageTableEntity> fixMessagesStorage,
            TranslatedSignalsRepository translatedSignalsRepository)
        {
            _logsStorage = logsStorage;
            _fixMessagesStorage = fixMessagesStorage;
            _translatedSignalsRepository = translatedSignalsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var query = new TableQuery<LogEntity>()
            {
                TakeCount = entriesCount
            };
            
            var logs = new List<LogEntity>(entriesCount);
            await _logsStorage.ExecuteAsync(query, result => logs.AddRange(result));

            var lastEntries = logs.OrderByDescending(x => x.Timestamp);
                
            return View(lastEntries);
        }

        public async Task<IActionResult> Fix()
        {
            var query = new TableQuery<FixMessageTableEntity>
            {
                TakeCount = entriesCount
            };

            var logs = new List<FixMessageTableEntity>(entriesCount);
            await _fixMessagesStorage.ExecuteAsync(query, result => logs.AddRange(result));

            var lastEntries = logs.OrderBy(x => x.RowKey);

            return View(lastEntries);
        }

        public async Task<IActionResult> TranslatedSignals()
        {
            return View(await _translatedSignalsRepository.GetTop(entriesCount));
        }
    }
}
