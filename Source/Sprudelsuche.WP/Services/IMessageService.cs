﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprudelsuche.WP.Services
{
    public interface IMessageService
    {
        Task ShowAsync(string content);
    }
}
