using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ElevaatorChallenge.Scratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var elevator = new Elevator() { Name = "E1" };
            var channel = Channel.CreateUnbounded<ElevatorRequest>();
            var tasks = new List<Task>();
            var requests = new ElevatorRequest[]
            {
                new ElevatorRequest() { FromFloor = 0, ToFloor = 5 },
                new ElevatorRequest() { FromFloor = 10, ToFloor = 0 },
            };
            tasks.Add(elevator.ElevatorRequestHandler(channel));
            tasks.Add(Producer(channel, requests));
            Task.WaitAll(tasks.ToArray());
        }

        static async Task Producer(ChannelWriter<ElevatorRequest> writer, IEnumerable<ElevatorRequest> requests, CancellationToken cancellationToken = default)
        {
            foreach (var req in requests)
                await writer.WriteAsync(req);
        }

        class Elevator
        {
            public string Name { get; set; }
            public int CurrentFloor { get; set; }
            public async Task ElevatorRequestHandler(ChannelReader<ElevatorRequest> reader)
            {
                await foreach (var req in reader.ReadAllAsync())
                {
                    if (CurrentFloor != req.FromFloor)
                        await MoveToAsync(new ElevatorRequest() { FromFloor = CurrentFloor, ToFloor = req.FromFloor });
                    await MoveToAsync(req);
                }
                await reader.Completion;
            }

            private async Task MoveToAsync(ElevatorRequest req)
            {
                if (req.Direction == Direction.Up)
                    for (int i = req.FromFloor; i <= req.ToFloor; i++)
                    {
                        CurrentFloor = i;
                        PrintCurrentFloor(Name, CurrentFloor, req.FromFloor, req.ToFloor);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                else
                    for (int i = req.FromFloor; i >= req.ToFloor; i--)
                    {
                        CurrentFloor = i;
                        PrintCurrentFloor(Name, CurrentFloor, req.FromFloor, req.ToFloor);
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
            }
        }

        static void PrintCurrentFloor(string elevatorName, int currentFloor, int fromFloor, int toFloor) =>
            Debug.WriteLine($"{elevatorName}: Current: {currentFloor} [{fromFloor}/{toFloor}]");

        class ElevatorRequest
        {
            public int FromFloor { get; set; }
            public int ToFloor { get; set; }
            public Direction Direction =>
                FromFloor > ToFloor ? Direction.Down : Direction.Up;
        }

        enum Direction
        {
            Up, Down
        }

        interface IRead<T>
        {
            Task<T> Read();
            bool IsComplete();
        }

        interface IWrite<T>
        {
            void Push(T msg);
            void Complete();
        }

        //class Channel<T> : IRead<T>, IWrite<T>
        //{
        //    private ConcurrentQueue<T> _queue;
        //    private SemaphoreSlim _flag;
        //    private bool _isFinished;

        //    public Channel()
        //    {
        //        _queue = new ConcurrentQueue<T>();
        //        _flag = new SemaphoreSlim(0);
        //    }

        //    public void Complete()
        //    {
        //        _isFinished = true && _queue.IsEmpty;
        //    }

        //    public bool IsComplete()
        //        => _isFinished;

        //    public void Push(T msg)
        //    {
        //        _queue.Enqueue(msg);
        //        _flag.Release();
        //    }

        //    public async Task<T> Read()
        //    {
        //        await _flag.WaitAsync();

        //        if (_queue.TryDequeue(out var msg))
        //            return msg;

        //        throw new NoItemOnQueueException();
        //    }

        //    class NoItemOnQueueException : Exception
        //    {
        //        public NoItemOnQueueException() : base("There is no item on the queue.")
        //        {
        //        }
        //    }
        //}
    }
}
