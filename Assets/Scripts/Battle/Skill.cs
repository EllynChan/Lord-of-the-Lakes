using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for skill types
public enum SKILL_TYPE
{
	ATK,
	HEAL,
	DEF,
	BUF,
	DEBUFF,
	PERCENTAGE_DMG,
	NONE // special cases, like run
}

public enum EFFECT_TYPE
{
	BUFF_ATK,
	BUFF_DEF,
	BUFF_SPD,
	DEBUFF_ATK,
	DEBUFF_DEF,
	DEBUFF_SPD,
	DEBUFF_ALL,
	NONE //no effect
};

// Effect struct to store buff/debuff information
[Serializable]
public struct Effect
{
	EFFECT_TYPE type;
	int num_rounds;
};

[CreateAssetMenu(fileName = "Skill", menuName ="Scriptable object/Item")]
public class Skill : ScriptableObject
{
	[Header("Label")]
	public string skill_name; // Skill name is unique
	public string skill_description;
	public SKILL_TYPE skill_type; // Skill type can be Attack, Buff, or Healing

	[Header("Stats")]
	public float skill_scale; // Skill scale factor
	public Effect skill_effect; // Reference to the Effect class
	public float effect_scale; // Effect scale factor
	public float probability; // This is for enemy AI
}
