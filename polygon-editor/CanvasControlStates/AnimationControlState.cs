using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace polygon_editor {
    public class AnimationControlState : CanvasControlState {
        Vec2[] Velocities;
        DispatcherTimer Timer;

        public AnimationControlState(CanvasState state) : base(state) {
            Timer = new DispatcherTimer();
        }

        private Vec2 AdjustVelocity(Vec2 velocity) {
            Vec2 minV = new Vec2(State.MinAnimationSpeed, State.MinAnimationSpeed);
            double range = State.MaxAnimationSpeed - State.MinAnimationSpeed;
            return velocity * range + minV;
        }

        public override void EnterState() {
            Random rnd = new Random();
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Start();
            Velocities = new Vec2[State.Polygons.Count];
            for(int i = 0; i < Velocities.Length; ++i) {
                Velocities[i] = Vec2.RandomNormal() * rnd.NextDouble();
            }
        }

        public override void ExitState() {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            foreach(Polygon polygon in State.Polygons) {
                for(int i = 0; i < polygon.Points.Length; ++i) {
                    
                }
            }
        }
    }
}
