# -*- coding: utf-8 -*-

"""Test clr enum support."""

import pytest

def test_reflectiononlyassembly_namespace(filepath):
    """Test accessing types in namespace in reflectiononly assembly."""
    import System.Reflection
    test_assembly=filepath("Python.ReflectionOnlyTest.dll")
    assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(test_assembly)
    import Python.ReflectionOnlyTest


def test_reflectiononlyassembly_global_namespace(filepath):
    """Test accessing types in global namespace in reflectiononly assembly."""    
    import System.Reflection
    test_assembly=filepath("Python.ReflectionOnlyTest.dll")
    assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(test_assembly)
    import clr