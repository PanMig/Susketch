using TileMapLogic;
using UnityEngine;
using UnityEngine.UI;

namespace Undo_Mechanism
{
    public class UndoRedoHandler : MonoBehaviour
    {
        [SerializeField] private Button _undoBtn;
        [SerializeField] private Button _redoBtn;
        private CommandInvoker _commandInvoker;
        public delegate void OnUndoRedo();
        public static event OnUndoRedo onUndoRedoEvent;

        void Start()
        {
            _commandInvoker = new CommandInvoker();
            EventManagerUI.onMapReadyForPrediction += AddMapEdit;
        }

        public void AddMapEdit()
        {
            var map = TileMap.GetMapDeepCopy(AuthoringTool.tileMapMain.GetTileMap());
            _commandInvoker.Execute(new TileMapEditCommand(map));
        }
        
        public void UndoEdit()
        {
            _commandInvoker.Undo();
            onUndoRedoEvent?.Invoke();
        }

        public void RedoEdit()
        {
            _commandInvoker.Redo();
            onUndoRedoEvent?.Invoke();
        }
    }
}