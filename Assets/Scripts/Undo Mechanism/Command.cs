using TileMapLogic;
using UnityEngine;

namespace Undo_Mechanism
{
    public abstract class Command
    {
        public virtual void Execute() {}
    }
}