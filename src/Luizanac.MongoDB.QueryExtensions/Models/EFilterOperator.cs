﻿namespace Luizanac.MongoDB.QueryExtensions.Models;

public enum EFilterOperator
{
	Equals,
	NotEquals,
	GreaterThan,
	LessThan,
	GreaterThanOrEqualTo,
	LessThanOrEqualTo,
	Contains,
	NotContains,
	StartsWith,
	NotStartsWith,
}