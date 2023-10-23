// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
//Version 1.3 - Tyler Wood
//               (Filled out skeleton code and added helper methods)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        //the graph for the dependent relationships
        private Dictionary<String, HashSet<String>> dependentGraph;
        //the graph for the dependee relationships
        private Dictionary<String, HashSet<String>> dependeeGraph;
        //the number of dependencies
        private int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependentGraph = new Dictionary<string, HashSet<string>>();
            dependeeGraph = new Dictionary<string, HashSet<string>>();
            size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get 
            {
                //As long as dependentgraph contains the key, return the size of the Hashset from that key. Otherwise return 0
                if (dependeeGraph.TryGetValue(s, out HashSet<string> result))
                {
                    return result.Count;
                }
                return 0; 
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (!dependentGraph.ContainsKey(s) || dependentGraph[s].Count == 0)
            {
                return false;
            }
            return true;
            
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (this[s] == 0)
            {
                return false;
            }
            return true;

        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            //As long as the dependentgraph contains the string s, return the hashset from that dependentgraph. Otherwise return an empty list
            if (!dependentGraph.ContainsKey(s))
            {
                return new HashSet<string>();
            }
            return dependentGraph[s];
            
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            //As long as the dependeegraph contains the string s, return the hashset from that dependeegraph. Otherwise return an empty list
            if (!dependeeGraph.ContainsKey(s))
            {
                return new HashSet<string>();
            }
            return dependeeGraph[s];
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            AddDependentHelper(s, t);
            AddDependeeHelper(s, t);
            
            
        }

        /// <summary>
        /// Assists with the adding of s and t to the dependent graph
        /// </summary>
        /// <param name="s"> the dependent of the ordered pair</param>
        /// <param name="t"> the dependee of the ordered pair</param>
        private void AddDependentHelper(string s, string t)
        {
            //Check to see if the dependent graph already contains s, then t. If it does, do nothing, if only s, then add t and increase size
            if (dependentGraph.ContainsKey(s))
            {
                if (!dependentGraph[s].Contains(t))
                {
                    dependentGraph[s].Add(t);
                    size++;
                }
            }

            //If s is not in the dependentgraph, then add both s and t
            else
            {
                dependentGraph.Add(s, new HashSet<string>());
                dependentGraph[s].Add(t);
                size++;
            }

            //Check to see if t is in the dependentgraph, if not then add it
            if (!dependentGraph.ContainsKey(t))
            {
                dependentGraph.Add(t, new HashSet<String>());
            }
        }

        /// <summary>
        /// Assists with the adding of s and t to the dependee graph
        /// </summary>
        /// <param name="s"> the dependent of the ordered pair</param>
        /// <param name="t"> the dependee of the ordered pair</param>
        private void AddDependeeHelper(string s, string t)
        {
            //Check to see if the dependee graph already contains t, then s. If it does, do nothing, if only t, then add s
            if (dependeeGraph.ContainsKey(t))
            {
                if (!dependeeGraph[t].Contains(s))
                {
                    dependeeGraph[t].Add(s);
                }

            }

            //Otherwise, add t and s to the dependee graph
            else
            {
                dependeeGraph.Add(t, new HashSet<string>());
                dependeeGraph[t].Add(s);
            }

            //Check to see if s is in the dependee graph. If not, add it
            if (!dependeeGraph.ContainsKey(s))
            {
                dependeeGraph.Add(s, new HashSet<String>());
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"> the dependent of the ordered pair</param>
        /// <param name="t"> the dependee of the ordered pair</param>
        public void RemoveDependency(string s, string t)
        {
            //Check to see if s, then t is in the dependent graph. If so, then remove it
            if (dependentGraph.ContainsKey(s))
            {
                if (dependentGraph[s].Contains(t))
                {
                    helpRemoveDependent(s, t);
                }
            }

            //Check to see if t, then s is in the dependee graph. If so, then remove it
            if (dependeeGraph.ContainsKey(t))
            {
                if (dependeeGraph[t].Contains(s))
                {
                    helpRemoveDependee(s, t);
                }
            }
        }

        /// <summary>
        /// Helper method that removes the dependent relationship from the graph, and the two points if they no longer have any dependents
        /// </summary>
        /// <param name="s"> the dependent of the ordered pair</param>
        /// <param name="t"> the dependee of the ordered pair</param>
        private void helpRemoveDependent(string s, string t)
        {
            //Remove the (s, t) pair from the dependent graph, and decrement size
            dependentGraph[s].Remove(t);
            size--;

            //If t's dependent set is empty, remove it from the graph
            if (dependentGraph.ContainsKey(t) && dependentGraph[t].Count == 0)
            {
                dependentGraph.Remove(t);
            }

            //If s' dependent set is now empty, remove it from the graph
            if (dependentGraph[s].Count == 0)
            {
                dependentGraph.Remove(s);
            }
        }

        /// <summary>
        /// Helper method that removes the dependee relationship from the graph, and the two points if they no longer have any dependees
        /// </summary>
        /// <param name="s"> the dependent of the ordered pair</param>
        /// <param name="t"> the dependee of the ordered pair</param>
        private void helpRemoveDependee(string s, string t)
        {
            //Remove the pair (t, s) from the dependee graph
            dependeeGraph[t].Remove(s);

            //If s' dependee set is empty, remove it from the graph
            if(dependeeGraph.ContainsKey(s) && dependeeGraph[s].Count == 0)
            {
                dependeeGraph.Remove(s);
            }

            //If t's dependee set is now empty, remove it from the graph
            if(dependeeGraph[t].Count == 0)
            {
                dependeeGraph.Remove(t);
            }

        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //Create a new HashSet to ensure that no enumerating is happening while the original set is being modified
            HashSet<string> remove = new HashSet<string>(GetDependents(s));

            //Go through each string within the new hashset, and remove the ordered pair (s, t) from the graph
            foreach (string t in remove)
            {
                RemoveDependency(s, t);
            }

            //Add each new dependency from the newDependents IEnumerable
            foreach (string t in newDependents)
            {
                AddDependency(s, t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //Create a new HashSet to ensure that no enumerating is happening while the original set is being modified
            HashSet<string> remove = new HashSet<string>(GetDependees(s));

            //Go through each string from the new hashset, and remove the ordered pair (t, s) from the graph
            foreach(string t in remove)
            {
                RemoveDependency(t, s);
            }

            //Add each new dependency from the newDependents IEnumerable
            foreach(string t in newDependees)
            {
                AddDependency(t, s);
            }
        }

    }

}
