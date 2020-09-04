using System.Collections.Generic;
using JetBrains.Annotations;
using Tensorflow;
using UnityEngine;

namespace Undo_Mechanism
{
    public class CommandInvoker
    {
        private const int HISTORY_SIZE = 10;
        private List<Command> _undoList = new List<Command>();
        private List<Command> _redoList = new List<Command>();

        public void Execute(Command command)
        {
            _undoList.Add(command);
            if (_redoList.Count > 0)
            {
                _redoList.Clear();
            }
            
            if(_undoList.Count >= HISTORY_SIZE)
            {
                _undoList.RemoveAt(0);
            }
            Debug.Log(_undoList.Count);
        }

        public void Undo()
        {
            if(_undoList.Count < 2) return;
            var currCommand = _undoList.Pop();
            _redoList.add(currCommand);
            _undoList.Peek().Execute();
        }

        public void Redo()
        {
            if(_redoList.Count < 2) return;
            var currCommand = _redoList.Pop();
            _undoList.add(currCommand);
            _redoList.Peek().Execute();
        }
    }
}

public static class ListExtensions
{
    public static void Push<T>(this List<T> myList, T elementToAdd)
    {
        myList.add(elementToAdd);
    }
    [CanBeNull]
    public static T Pop<T>(this List<T> myList)
    {
        var topIndex = myList.Count - 1;
        T element = myList[topIndex];
        myList.RemoveAt(topIndex);
        return element;
    }
    
    public static T PopAtIndex<T>(this List<T> myList, int index)
    {
        T element = myList[index];
        myList.RemoveAt(index);
        return element;
    }
    
    public static T Peek<T>(this List<T> myList)
    {
        return myList[myList.Count - 1];
    }
}

// STACK IMPLEMENTATION
// private Stack<Command> _undoStack = new Stack<Command>();
// private Stack<Command> _redoStack = new Stack<Command>();
// private const int STACK_MAX_SIZE = 10;
//
// public void Execute(Command command)
// {
// _undoStack.Push(command);
// if (_redoStack.Count != 0)
// {
//     _redoStack.Clear();
// }
// }
//
// public void Undo()
// {
// if (_undoStack.Count == 0) return;
// // remove from undo stack and push to redo stack.
// var currCommand =  _undoStack.Pop();
// _redoStack.Push(currCommand);
// // execute the previous command.
// var undoCommand = _undoStack.Peek();
// undoCommand.Execute();
// }
//
// public void Redo()
// {
// if (_redoStack.Count == 0) return;
// // remove from redo stack and push to undo stack.
// var currCommand =  _redoStack.Pop();
// _undoStack.Push(currCommand);
// // execute the previous command.
// var redoCommand = _redoStack.Peek();
// redoCommand.Execute();
// }