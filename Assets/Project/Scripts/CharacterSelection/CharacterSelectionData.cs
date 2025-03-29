using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterSelectionData
{
	public static BaseCharacter SelectedPlayerCharacter { get; set; }
	public static BaseCharacter SelectedOpponentCharacter { get; set; }

	public static void Clear()
	{
		SelectedOpponentCharacter = null;
		SelectedPlayerCharacter = null;
	}
}
