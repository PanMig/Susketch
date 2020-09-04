using TileMapLogic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Undo_Mechanism
{
    public class TileMapEditCommand : Command
    {
        private Tile[,] _map;

        public TileMapEditCommand(Tile[,] map)
        {
            this._map = map;
        }
        
        public override void Execute()
        {
            AuthoringTool.tileMapMain.CopyTileMap(_map);
            AuthoringTool.tileMapMain.Render();
            AuthoringTool.SetTileOrientation();
        }
    }
}