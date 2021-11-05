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
        Random Rnd;

        public AnimationControlState(CanvasState state) : base(state) {
            Timer = new DispatcherTimer();
            Rnd = new Random();
        }

        private Vec2 AdjustVelocity(Vec2 velocity) {
            if(velocity.Length() < State.MinAnimationSpeed) {
                velocity *= State.MinAnimationSpeed / velocity.Length();
            }
            if(velocity.Length() > State.MaxAnimationSpeed) {
                velocity *= State.MaxAnimationSpeed / velocity.Length();
            }
            return 20 * velocity;
        }

        private Vec2 RandomVelocity() {
            return Vec2.RandomNormal() * Rnd.NextDouble();
        }

        public override void EnterState() {
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 20);
            Timer.Start();
            Velocities = new Vec2[State.Polygons.Count];
            for(int i = 0; i < Velocities.Length; ++i) {
                Velocities[i] = RandomVelocity();
            }
        }

        public override void ExitState() {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            for(int i = 0; i < State.Polygons.Count; ++i) {
                Polygon poly = State.Polygons[i];
                for(int j = 0; j < poly.Points.Length; ++j) {
                    poly.Points[j] += AdjustVelocity(Velocities[i]);
                }

                for(int j = 0; j < poly.Points.Length; ++j) {
                    if(poly.Points[j].X >= State.Canvas.Width) {
                        poly.Translate(new Vec2(State.Canvas.Width - poly.Points[j].X - 1, 0));
                        Velocities[i] = RandomVelocity();
                        break;
                    }
                    if (poly.Points[j].X < 0) {
                        poly.Translate(new Vec2(-poly.Points[j].X, 0));
                        Velocities[i] = RandomVelocity();
                        break;
                    }
                    if (poly.Points[j].Y >= State.Canvas.Height) {
                        poly.Points[j].Y = State.Canvas.Height - 1;
                        poly.Translate(new Vec2(0, State.Canvas.Height - poly.Points[j].Y - 1));
                        Velocities[i] = RandomVelocity();
                        break;
                    }
                    if (poly.Points[j].Y < 0) {
                        poly.Translate(new Vec2(0, -poly.Points[j].Y));
                        Velocities[i] = RandomVelocity();
                        break;
                    }
                }
            }

            State.UpdateCanvas();
        }
    }
}
