﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.FileInUseCheck
{
    public interface IFileInUseChecker
    {
        bool IsFileInUse(string filePath);
    }
}