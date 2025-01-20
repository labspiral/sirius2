using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demos
{

    public enum StageStatus
    {
        Ready,
        Busy,
        Error,
        Stopped,
    }

    public interface IStageXy
    {
        StageStatus Status { get; }

        bool MoveAbs(System.Numerics.Vector2 position);

        bool MoveRel(System.Numerics.Vector2 distance);

        bool Stop();

        void Reset();

    }
}
