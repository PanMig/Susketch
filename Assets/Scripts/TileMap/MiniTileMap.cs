using TileMapLogic;
using UnityEngine;

public class MiniTileMap : TileMap
{
    private static readonly int rows = 20;
    public static readonly int columns = 20;

    public override void Init()
    {
        tileMap = new Tile[rows, columns];
        TileThemes tileTheme;
        Decoration dec;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                //ground tile and empty decoration.
                tileTheme = Brush.Instance.miniMapThemes[0];
                dec = Brush.Instance.decorations[0];
                tileMap[row, col] = new Tile(tileTheme.envTileID, dec.decorationID, row, col);
            }
        }
    }

    public override void PaintTiles(Transform parent, float decorationScale)
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                tileMap[i, j].PaintTile(Brush.Instance.miniMapThemes[0].prefab,
                    Brush.Instance.brushThemes[0], Brush.Instance.decorations[0], parent, decorationScale);
            }
        }
    }
}
