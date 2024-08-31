using System.Collections.Concurrent;

namespace ImageManipulationSystemDrawing;

public class SorterGroup
{
    private SpanSorter[] agents;
    private Thread[] agentThreads;
    private volatile bool cancelToken;

    public SorterGroup(ConcurrentQueue<SortingTask> spansToSort, ConcurrentQueue<SortingTask> completedSpans, int nAgents)
    {
        cancelToken = false;
        agents = new SpanSorter[nAgents];
        for (int i = 0; i < nAgents; i++)
        {
            agents[i] = new SpanSorter(spansToSort, completedSpans, cancelToken);
        }

        agentThreads = new Thread[nAgents];
        for (int i = 0; i < nAgents; i++)
        {
            agentThreads[i] = new Thread(agents[i].SortSpans);
        }
    }

    public void start()
    {
        foreach (Thread agentThread in agentThreads)
        {
            agentThread.Start();
        }
    }

    public void stop()
    {
        cancelToken = true;
    }
}