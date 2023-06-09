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
        void Execute(string[] args);
    }

    public interface IUndoable : ICommand
    {
        void Undo();
        void Redo();
    }
}
