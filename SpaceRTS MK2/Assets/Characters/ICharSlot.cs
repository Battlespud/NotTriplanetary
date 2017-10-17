using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharSlot {

	List<Character> GetCharacters();
	bool TransferCharacterTo (Character c);
	void TransferCharacter(Character c, ICharSlot targ);
}
