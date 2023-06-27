using MiceControls;
using ODC.P5Sharp;
using System.Numerics;

namespace WanderingMouse
{
    /// <summary>
    /// The screen size is currently hard coded on 1920*1080
    /// Change it in the mehtod Edges() and also in the MiceControlsLib "MouseControls.cs" mappedX = .... {your resolution}
    /// As you can see, it is a NapkinSketch, I have created it about 4 years ago (2k20) as I started coding.
    /// You will be able to exit the program with any key. See Console.CancelKeyPress event.
    /// I did not refactored the code to work out of the box on everyones device.
    /// </summary>
    internal class Program
    {
        static void Main()
        {
            FireUpTasks();
        }
        public static void FireUpTasks()
        {
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Exiting");
                Console.ReadLine();
                Environment.Exit(0);
            };
            var taskKeys = new Task(ReadKeys);
            var taskMoveMouse = new Task(Setup);

            taskKeys.Start();
            taskMoveMouse.Start();
            var tasks = new[] { taskKeys };
            Task.WaitAll(tasks);
        }

        public static void Setup()
        {
            Vehicle a = new(MouseControls.GetCursorPosition().X, MouseControls.GetCursorPosition().Y);
            Thread.Sleep(2000);
            Console.WriteLine("start");
            while (a.Pos != new Vector2(600, 600))
            {
                a.Wander();
                a.Update();
                a.Edges();
                MouseControls.LeftDownAndMove((int)a.Pos.X, (int)a.Pos.Y);
                Thread.Sleep(15);
                Console.WriteLine(a.Pos);
            }
            Console.WriteLine("End");
        }
        private static void ReadKeys()
        {
            ConsoleKeyInfo key = new();
            while (!Console.KeyAvailable && key.Key != ConsoleKey.Escape)
            {
                key = Console.ReadKey(true);
            }
        }
    }
    class Vehicle
    {
        #region variables
        Random rnd = new();

        private bool hitEdge;

        public bool HitEdge { get => hitEdge; set => hitEdge = value; }

        private bool alreadyPressed;
        public bool AlreadyPressed { get => alreadyPressed; set => alreadyPressed = value; }

        private Vector2 pos;
        public Vector2 Pos { get => pos; set => pos = value; }

        private Vector2 vel;
        public Vector2 Vel { get => vel; set => vel = value; }

        private Vector2 acc;
        public Vector2 Acc { get => acc; set => acc = value; }

        private float maxForce;
        public float MaxForce { get => maxForce; set => maxForce = value; }

        private float maxSpeed;
        public float MaxSpeed { get => maxSpeed; set => maxSpeed = value; }

        private int r;
        public int R { get => r; set => r = value; }

        private float wanderTheta;
        public float WanderTheta { get => wanderTheta; set => wanderTheta = value; }

        private List<Vector2> currentPath = new();
        public List<Vector2> CurrentPath { get => currentPath; set => currentPath = value; }

        private List<Vector2> paths;
        public List<Vector2> Paths { get => paths; set => paths = value; }
        #endregion

        public Vehicle(int x, int y)
        {
            pos = new Vector2(x, y);
            vel = new Vector2(0, 0);
            acc = new Vector2(0, 0);
            maxSpeed = 10f;
            maxForce = 0.5f;
            r = 16;
            vel = vel.SetMag(maxSpeed);
            wanderTheta = -(MathF.PI / 2);
            currentPath.Clear();
            paths = currentPath;

        }
        public void Wander()
        {
            Vector2 wanderPoint = vel;
            wanderPoint = wanderPoint.SetMag(100);
            wanderPoint = Vector2.Add(pos, wanderPoint);

            float wanderRadius = 50;

            float theta = wanderTheta + vel.Heading();
            float x = wanderRadius * MathF.Cos(theta);
            float y = wanderRadius * MathF.Sin(theta);

            wanderPoint = wanderPoint.MyAdd(x, y);

            var steer = Vector2.Subtract(wanderPoint, pos);
            steer = steer.Limit(rnd.Next(1, 14)); //maxForce
            ApplyForce(steer);

            rnd = new Random();
            var selector = rnd.NextDouble();


            if (selector < 0.5f)
            {
                wanderTheta += -0.3f;
            }
            else if (selector >= 0.5f)
            {
                wanderTheta += 0.3f;
            }
            maxSpeed = rnd.Next(1, 15);

        }
        public void Wander2()
        {
            var wanderPoint = new Vector2(pos.X, pos.Y);
            wanderPoint = wanderPoint.SetMag(100);
            wanderPoint = Vector2.Add(wanderPoint, pos);

            var wanderRadius = 50;
            var theta = wanderTheta + vel.Heading();

            var x = wanderRadius * Math.Cos(theta);
            var y = wanderRadius * Math.Sin(theta);
            wanderPoint = new Vector2(wanderPoint.X + (float)x, wanderPoint.Y + (float)y);

            var steer = Vector2.Subtract(wanderPoint, pos);
            steer = steer.SetMag(maxForce);
            steer.Limit(maxForce);
            ApplyForce(steer);

            var selector = rnd.NextDouble();
            if (selector < 0)
            {
                wanderTheta += -0.3f;

            }
            else if (selector >= 0)
            {
                wanderTheta += 0.3f;
            }

        }
        public void Seek(Vector2 target)
        {
            Vector2 force = Vector2.Subtract(target, pos);
            force = force.SetMag(maxSpeed); //
            force = Vector2.Subtract(force, vel);
            force.Limit((int)maxForce);
            ApplyForce(force);
        }
        public void Arrive(Vector2 target)
        {
            var desired = Vector2.Subtract(target, pos);
            var d = Math.Sqrt(desired.MagSq());
            if (d < 100)
            {
                var m = MouseControls.Remap((float)d, 0, 200, 0, maxSpeed);
                desired = desired.SetMag(m);
            }
            else
            {
                desired = desired.SetMag(maxSpeed);
            }

            //
            var steer = Vector2.Subtract(desired, vel);
            steer.Limit(maxForce);
            ApplyForce(steer);
        }
        public void ApplyForce(Vector2 force)
        {
            acc = Vector2.Add(acc, force);
        }

        public void Update()
        {

            vel = Vector2.Add(vel, acc);
            vel = vel.Limit(maxSpeed);
            //pos = new(MouseControls.GetCursorPosition().X, MouseControls.GetCursorPosition().Y);
            pos = Vector2.Add(pos, vel);
            acc = new Vector2(0, 0);

            //currentPath.Add(pos);
        }

        public void Edges()
        {
            hitEdge = false;
            //if (pos.X > 1920 + r)
            //{
            //    pos.X = -r;
            //    hitEdge = true;

            //}
            //else if (pos.X < -r)
            //{
            //    pos.X = 1920 + r;
            //    hitEdge = true;

            //}
            //if (pos.Y > 1080 + r)
            //{

            //    pos.Y = -r;
            //    hitEdge = true;
            //}
            //else if (pos.Y < -r)
            //{
            //    pos.Y = 1080 + r;
            //    hitEdge = true;

            //}
            if (pos.X > 1920 || pos.X <= 0 || pos.Y > 1080 || pos.Y <= 0)
            {
                //MouseControls.LeftDown(false);
                hitEdge = true;
                pos.X = rnd.Next(50, 1870);
                pos.Y = rnd.Next(200, 600);
            }
            //if (hitEdge)
            //{
            //    //currentPath.Clear();
            //    //paths = currentPath;
            //}
        }
    }
}