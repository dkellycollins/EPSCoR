﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.Database.Services.Log
{
    public class DbLogger : ILogger
    {
        IDbContextFactory _contextFactory;

        public DbLogger()
        {
            _contextFactory = new DbContextFactory();
        }

        public void Log(string message)
        {
            if (message.Length > 100)
                message = message.Substring(0, 97) + "...";

            using (ModelDbContext context = _contextFactory.GetModelDbContext())
            {
                context.CreateModel(new LogEntry()
                {
                    Message = message
                });
            }
        }

        public void Log(string message, Exception e)
        {
            if (message.Length > 100)
                message = message.Substring(0, 97) + "...";

            string errMessage = e.ToString();
            if (errMessage.Length > 100)
                errMessage = errMessage.Substring(0, 97) + "...";

            using (ModelDbContext context = _contextFactory.GetModelDbContext())
            {
                context.CreateModel(new LogEntry()
                {
                    Message = message,
                    Error = errMessage
                });
            }
        }
    }
}