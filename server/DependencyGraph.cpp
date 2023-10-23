/* COCOPRO Networked Spreadsheet Final Project 
 * 
 * spreadsheet DependencyGraph class definitions
 */

#include "DependencyGraph.h"
#include <vector>
#include <string>
#include <unordered_map>
#include <unordered_set>
#include <iostream> 

DependencyGraph::DependencyGraph ()
{
  dependentGraph = {};
  dependeeGraph = {};
  size = 0;
}

int DependencyGraph::getSize () const 
{
  return size;
}

bool DependencyGraph::hasDependents(std::string s)
{
  if (!dependentContains(s) || dependentGraph[s].size() == 0)
    return false;
  return true;
}

bool DependencyGraph::hasDependees(std::string s)
{
  if (!dependeeContains(s) || dependeeGraph[s].size() == 0)
    return false;
  return true;
}

std::vector<std::string> DependencyGraph::getDependents(std::string s)
{
  //As long as the dependentgraph contains the string s, return the hashset from that dependentgraph. Otherwise return an empty list
  if (!dependentContains(s))
    return std::vector<std::string>();
  std::vector<std::string> output(dependentGraph[s].begin(), dependentGraph[s].end());
  return output;
}

std::vector<std::string> DependencyGraph::getDependees(std::string s)
{
  //As long as the dependeegraph contains the string s, return the hashset from that dependeegraph. Otherwise return an empty list
  if (!dependeeContains(s))
    return std::vector<std::string>();
  std::vector<std::string> output(dependeeGraph[s].begin(), dependeeGraph[s].end());
  return output;
}

void DependencyGraph::addDependency(std::string s, std::string t)
{
  addDependentHelper(s, t);
  addDependeeHelper(s, t);
}

void DependencyGraph::removeDependency(std::string s, std::string t)
{
  //Check to see if s, then t is in the dependent graph. If so, then remove it
  if (dependentContains(s))
  {
    if (dependentGraph[s].find(t) != dependentGraph[s].end())
    {
      helpRemoveDependent(s, t);
    }
  }

  //Check to see if t, then s is in the dependee graph. If so, then remove it
  if (dependeeContains(t))
  {
    if (dependeeGraph[t].find(t) != dependeeGraph[s].end())
    {
      helpRemoveDependee(s, t);
    }
  }
}

void DependencyGraph::replaceDependents(std::string s, std::vector<std::string> dependents)
{
  //Create a  HashSet to ensure that no enumerating is happening while the original set is being modified
  std::vector<std::string> remove(getDependents(s));

  //Go through each string within the  hashset, and remove the ordered pair (s, t) from the graph
  
  for (int i = 0; i < remove.size(); i++)
  {
    removeDependency(s, remove[i]);
  }

  //Add each  dependency from the Dependents vector
  for (int i = 0; i < dependents.size(); i++)
  {
	addDependency(s, dependents[i]);
  }
  
  
}

void DependencyGraph::replaceDependees(std::string s, std::vector<std::string> dependees)
{
  //Create a  HashSet to ensure that no enumerating is happening while the original set is being modified
  std::vector<std::string> remove(getDependees(s));
  //Go through each string from the  hashset, and remove the ordered pair (t, s) from the graph
  for (int i = 0; i < remove.size(); i++)
  {
    removeDependency(remove[i], s);
  }

if(dependees.size() != 0)
{
  //Add each  dependency from the Dependents vector
  for (int i = 0; i < dependees.size(); i++)
  {
    addDependency(dependees[i], s);
  }
}
  
}

void DependencyGraph::addDependentHelper(std::string s, std::string t)
{
  //Check to see if the dependent graph already contains s, then t. If it does, do nothing, if only s, then add t and increase size
  if (dependentContains(s))
  {
    if (dependentGraph[s].find(t) == dependentGraph[s].end())
    {
      dependentGraph[s].insert(t);
      size++;
    }
  }

  //If s is not in the dependentgraph, then add both s and t
  else
  {
    dependentGraph.insert(std::pair<std::string, std::unordered_set<std::string>>(s, std::unordered_set<std::string>()));
    dependentGraph[s].insert(t);
    size++;
  }

  //Check to see if t is in the dependentgraph, if not then add it
  if (!dependentContains(t))
  {
    dependentGraph.insert(std::pair<std::string, std::unordered_set<std::string>>(t, std::unordered_set<std::string>()));
  }
}

void DependencyGraph::addDependeeHelper(std::string s, std::string t)
{
  //Check to see if the dependee graph already contains t, then s. If it does, do nothing, if only t, then add s
  if (dependeeContains(t))
  {
    if (dependeeGraph[t].find(t) == dependeeGraph[s].end())
    {
      dependeeGraph[t].insert(s);
    }

  }

  //Otherwise, add t and s to the dependee graph
  else
  {
    dependeeGraph.insert(std::pair<std::string, std::unordered_set<std::string>>(t,  std::unordered_set<std::string>()));
    dependeeGraph[t].insert(s);
  }

  //Check to see if s is in the dependee graph. If not, add it
  if (!dependeeContains(s))
  {
    dependeeGraph.insert(std::pair<std::string, std::unordered_set<std::string>>(s, std::unordered_set<std::string>()));
  }
}

void DependencyGraph::helpRemoveDependent(std::string s, std::string t)
{
  //Remove the pair (t, s) from the dependent graph
  dependentGraph[t].erase(s);

  //If s' dependent set is empty, remove it from the graph
  if(dependentContains(s) && dependentGraph[s].size() == 0)
  {
    dependentGraph.erase(s);
  }

  //If t's dependent set is now empty, remove it from the graph
  if(dependentGraph[t].size() == 0)
  {
    dependentGraph.erase(t);
  }
}

void DependencyGraph::helpRemoveDependee(std::string s, std::string t)
{
  //Remove the pair (t, s) from the dependee graph
  dependeeGraph[t].erase(s);

  //If s' dependee set is empty, remove it from the graph
  if(dependeeContains(s) && dependeeGraph[s].size() == 0)
  {
    dependeeGraph.erase(s);
  }

  //If t's dependee set is now empty, remove it from the graph
  if(dependeeGraph[t].size() == 0)
  {
    dependeeGraph.erase(t);
  }
}

bool DependencyGraph::dependentContains (std::string s)
{
  auto search = dependentGraph.find(s);
  return (search != dependentGraph.end());
}

bool DependencyGraph::dependeeContains (std::string s)
{
  auto search = dependeeGraph.find(s);
  return (search != dependeeGraph.end());
}

