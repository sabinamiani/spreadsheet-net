#ifndef DEP_GRAPH_H
#define DEP_GRAPH_H

#include <string>
#include <vector>
#include <unordered_map>
#include <unordered_set>

class DependencyGraph
{

  //the graph for the dependent relationships
  std::unordered_map<std::string, std::unordered_set<std::string>> dependentGraph; 
  //the graph for the dependee relationships
  std::unordered_map<std::string, std::unordered_set<std::string>> dependeeGraph;
  //the number of dependencies
  int size;

 public:

  /// constructor ///

  // Creates an empty DependencyGraph.
  DependencyGraph();

  /// methods ///

  // size getter
  int getSize () const;

  // Reports whether dependents(s) is non-empty.
  bool hasDependents(std::string s); 

  // Reports whether dependees(s) is non-empty.
  bool hasDependees(std::string s); 

  // Enumerates dependents(s).
  std::vector<std::string> getDependents(std::string s);

  // Enumerates dependees(s).
  std::vector<std::string> getDependees(std::string s);

  // Adds the ordered pair (s,t), if it doesn't exist
  // This should be thought of as: t depends on s
  void addDependency(std::string s, std::string t);

  // Removes the ordered pair (s,t), if it exists
  void removeDependency(std::string s, std::string t);

  // Removes all existing ordered pairs of the form (s,r).  Then, for each
  //  t in newDependents, adds the ordered pair (s,t).
  void replaceDependents(std::string s, std::vector<std::string> newDependents);

  // Removes all existing ordered pairs of the form (r,s).  Then, for each 
  //  t in newDependees, adds the ordered pair (t,s).
  void replaceDependees(std::string s, std::vector<std::string> newDependees);

  bool dependentContains (std::string s);

  bool dependeeContains (std::string s);

 private:
  
  // Assists with the adding of s and t to the dependent graph
  void addDependentHelper(std::string s, std::string t);

  // Assists with the adding of s and t to the dependee graph
  void addDependeeHelper(std::string s, std::string t);

  // Helper method that removes the dependent relationship from the graph, 
  //  and the two points if they no longer have any dependents
  void helpRemoveDependent(std::string s, std::string t);

  // Helper method that removes the dependee relationship from the graph, 
  //  and the two points if they no longer have any dependees
  void helpRemoveDependee(std::string s, std::string t);

};

#endif