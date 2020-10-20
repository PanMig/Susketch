using System.Collections.Generic;
using Tensorflow;
using UnityEngine;

namespace Undo_Mechanism
{
    public class CommandInvoker
    {
        private const int HISTORY_SIZE = 20;
        private List<Command> _undoList = new List<Command>();
        private List<Command> _redoList = new List<Command>();

        public void Execute(Command command)
        {
            _undoList.Add(command);
            Debug.Log("adding");
            if (_redoList.Count > 0)
            {
                _redoList.Clear();
            }
            
            if(_undoList.Count == HISTORY_SIZE)
            {
                _undoList.RemoveAt(0);
            }
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