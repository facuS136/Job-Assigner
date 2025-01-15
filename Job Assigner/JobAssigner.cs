using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms.MaximumFlow;

namespace Job_Assigner
{
    // We will use Edmonds-Karp algorithm to make a job assignation using graphs.
    public static class JobAssigner
    {
        public static string DoAssignation(Dictionary<string, int> employees, Dictionary<string, int> jobs, Dictionary<string, List<string>> employeeJobs)
        {

            List <Edge<string>> employeesEdges = new List<Edge<string>>();
            List<Edge<string>> employeesJobsEdges = new List<Edge<string>>();
            List<Edge<string>> jobsEdges = new List<Edge<string>>();

            // we construct each edge for the graph
            foreach (string employee in employees.Keys) 
            {
                employeesEdges.Add(new Edge<string>("BEGINING", employee));

                foreach (string job in employeeJobs[employee])
                {
                    employeesJobsEdges.Add(new Edge<string>(employee, job));
                }
            }

            foreach (string job in jobs.Keys)
            {
                jobsEdges.Add(new Edge<string>(job, "FINISH"));
            }

            // Now lets join add each edge to the graph and assing them a capacitie of flow
            Dictionary<Edge<string>, double> capacities = new Dictionary<Edge<string>, double>();       // a capacities dictionary
            AdjacencyGraph<string, Edge<string>> graph = new AdjacencyGraph<string, Edge<string>>();    // a graph

            foreach (Edge<string> edge in employeesEdges)
            {
                graph.AddVerticesAndEdge(edge);
                capacities[edge] = employees[edge.Target];
            }

            foreach (Edge<string> edge in employeesJobsEdges)
            {
                graph.AddVerticesAndEdge(edge);
                capacities[edge] = 1;
            }

            foreach (Edge<string> edge in jobsEdges)
            {
                graph.AddVerticesAndEdge(edge);
                capacities[edge] = jobs[edge.Source];
            }

            // now lets run the algorithm
            var maxFlowAlgorithm = new EdmondsKarpMaximumFlowAlgorithm<string, Edge<string>>(
                graph, 
                (e) => capacities[e], 
                new EdgeFactory<string, Edge<string>>(
                    (source, target) => new Edge<string>(source, target)
                    )
                );

            maxFlowAlgorithm.Compute("BEGINING", "FINISH"); // we compute it

            Dictionary<string, string> assignations = new Dictionary<string, string>();

            // in a dictionary we will save for each employee an string separated by "," with the jobs assigned to the employee. Example:  {"James" : "Cleaning, Reception, ..., etc"}
            foreach (Edge<string> edge in employeesJobsEdges)
            {
                if (maxFlowAlgorithm.ResidualCapacities[edge] == 0)
                {
                    if (!assignations.ContainsKey(edge.Source))
                    {
                        assignations[edge.Source] = edge.Target;
                    }
                    else
                    {
                        assignations[edge.Source] += ", " + edge.Target;
                    }
                }
            }

            string output = "";

            foreach (string employee in assignations.Keys)
            {
                output += employee + " : " + assignations[employee] + "\n";
            }

            return output;
        }
    }
}


