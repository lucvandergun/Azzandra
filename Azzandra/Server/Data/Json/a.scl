import Diagnosis._
import Conflicts.tpf
import gapt.expr.formula.Formula
import gapt.expr.stringInterpolationForExpressions

import scala.Option
import scala.collection.mutable.Stack
import gapt.expr.formula.fol.FOLTerm

object Main extends App {

  // Definition of a Tree
  sealed trait Tree[+A]
  case class Root[A](children: List[Tree[A]]) extends Tree[A]
  case class Leaf[A](value: A) extends Tree[A]
  case class Branch[A](value: A, children: List[Tree[A]]) extends Tree[A]

  def makeHittingTree[A](setList: List[Set[A]]) : Tree[A] = {
    // Takes a list of sets of arbitrary elements and creates a tree

    def _makeHittingTree[A](setList: List[Set[A]]) : List[Tree[A]] = setList match {
      // Creates a list of leaves or branches depending on the set list size
      case x::Nil => _createLeaves(x.toList)
      case x::xs => _createNodes(x.toList, xs)
    }

    def _createLeaves[A](list: List[A]) : List[Tree[A]] = list match {
      // Creates a list of leaves given a list of arbitrary elements
      case Nil => Nil
      case x::xs => Leaf(x) :: _createLeaves(xs)
    }

    def _createNodes[A](list: List[A], setList: List[Set[A]]) : List[Tree[A]] = list match {
      // Creates a list of branches given a list of arbitrary elements and a set list
      case Nil => Nil
      case x::xs => Branch(x, _makeHittingTree(setList)) :: _createNodes(xs, setList)
    }

    Root(_makeHittingTree(setList))
  }

  def makeHittingTreeTpf(problem: () => (List[Formula], List[FOLTerm], List[Formula])) : Tree[FOLTerm] = {
    // Takes a problem and creates a tree using tpf on the fly
    
    def _makeHittingTree(cf: List[FOLTerm], faulty: List[FOLTerm]) : List[Tree[FOLTerm]] = cf match {
      // Takes a conflict set and list of faulty components and creates a child for each component.
      case Nil => List()
      case c :: xs => _createChild(c, faulty) :: _makeHittingTree(xs, faulty)
    }
    
    def _createChild (c: FOLTerm, faulty: List[FOLTerm]) : Tree[FOLTerm] = tpf(problem, c::faulty) match {
      // Creates a Leaf if tpf does not find a new conflict after adding c to the list of faulty components.
      // If it does find a set it creates a branch with new children created by _makeHittingTree
      case None => Leaf(c)
      case Some(cf) => Branch(c, _makeHittingTree(cf.toList, c::faulty))
    }
    
    tpf(problem, List()) match {
      case None => Root(Nil) // If the provided system definition is correct it returns an empty node
      case Some(cf) => Root(_makeHittingTree(cf.toList, List()))
    }
  }

  def gatherHittingSets[A](tree: Tree[A]) : List[Set[A]] = {
    // Read the given tree for all hitting sets:

    def _gatherHittingSets(tree: Tree[A], current: Set[A]) : List[Set[A]] = tree match {
      // gathers the hitting sets given a tree and set of components that it is currently gathering
      case Root(children) => _gatherChildren(children, current)
      case Leaf(e) => List(current + e)
      case Branch(e, children) => _gatherChildren(children, current + e)
    }
    
    def _gatherChildren(children: List[Tree[A]], current: Set[A]) : List[Set[A]] = children match {
      // gathers the hitting sets of a list of children given a set it is currently gathering
      case Nil => Nil
      case x::xs => _gatherHittingSets(x, current) ++ _gatherChildren(xs, current)
    }

    _gatherHittingSets(tree, Set[A]())
  }

  def getDiagnoses[A](hittingSets: List[Set[A]]): List[Set[A]] = {
    // Remove all supersets from the given list:
    hittingSets.distinct.filter(s => hittingSets.forall(x => !x.subsetOf(s) || s == x))
  }


  def findDiagnoses(problem: () => (List[Formula], List[FOLTerm], List[Formula])) = {
    // Input a diagnostic problem and find all minimal hitting sets (i.e. diagnoses) for it:

    def _findConflictSets(problem: () => (List[Formula], List[FOLTerm], List[Formula]), hs: List[FOLTerm]): List[Set[FOLTerm]] = {
      tpf(problem, hs) match {
        case None => Nil
        case Some(cs) => { cs ::
          cs.toList.flatMap(c => _findConflictSets(problem, c :: hs))
        }
      }
    }

    // val conflictSets = _findConflictSets(problem, List())
    // println("Conflict sets: " + conflictSets)
    // val tree = makeHittingTree(conflictSets)
    val tree = makeHittingTreeTpf(problem)
    println("Tree: " + tree)
    val hittingSets = gatherHittingSets(tree)
    println("Hitting sets: " + hittingSets)
    getDiagnoses(hittingSets)
  }

  println("Running diagnostics on problem...")
  println("Minimal hitting sets: " + findDiagnoses(problem3))
}