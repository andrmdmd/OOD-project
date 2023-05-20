﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }

        string Usage { get; }
        string[] Prepare(string[] args);
        void Execute(string[] args);
    }
}
