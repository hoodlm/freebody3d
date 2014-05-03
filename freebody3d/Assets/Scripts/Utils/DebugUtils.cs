#define ASSERT_ON

using System.Diagnostics;
using System;
using UnityEngine;

/**
 * 	A simple assertion class, based on Steve9's answer at:
 * 	http://answers.unity3d.com/questions/19122/assert-function.html
 */
public static class DebugUtils {

	[Conditional("ASSERT_ON")]
	/**
	 *	Throws an exception if the condition is true.
	 *	@param condition The condition to test.
	 */
	public static void Assert(bool condition)
	{
		if (!condition) throw new Exception();
	}

	[Conditional("ASSERT_ON")]
	/**
	 *	Throws an exception if the condition is true.
	 *	@param condition The condition to test.
	 *	@param message A message to be included in the exception.
	 */
	public static void Assert(bool condition, string message)
	{
		if (!condition) throw new Exception(message);
	}

	[Conditional("ASSERT_ON")]
	/**
	 *	Throws an exception if the condition is true.
	 *	@param condition The condition to test.
	 *	@param message A message to be included in the exception.
	 *	@param caller The object that made the assertion.
	 */
	public static void Assert(bool condition, string message, object caller)
	{
		string exceptionMessage = string.Format("{0}: {1}", caller.GetType().Name, message);
		if (!condition) throw new Exception(exceptionMessage);
	}
}