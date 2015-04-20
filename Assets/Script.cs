using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Script : MonoBehaviour
{
	Toggle[] toggles;
	int length;
	uint bitMask;
	bool sendCallback = true;

	void Start()
	{
		bitMask = (uint)1;

		toggles = GameObject.FindObjectsOfType<Toggle>();
		foreach (var item in toggles)
			item.onValueChanged.AddListener(GetToggleHandler(item));

		System.Array.Sort(toggles, (lhs, rhs) => { return lhs.transform.GetSiblingIndex().CompareTo(rhs.transform.GetSiblingIndex()); });

		length = toggles.Length;

		if (Application.isWebPlayer)
		{
			Application.ExternalCall("Start", gameObject.name);
			Application.ExternalCall("GetHash");
		}

		//int i = 3;
		//BitArray b = new BitArray(new int[] { i });
		//Debug.Log(b.Length);
		//foreach (var item in b)
		//	Debug.Log(item);

		//string s = Convert.ToString(i, 2);
		//Debug.LogError(s);
	}

	private UnityEngine.Events.UnityAction<bool> GetToggleHandler(Toggle item)
	{
		return new UnityEngine.Events.UnityAction<bool>((b) =>
		{
			if (sendCallback)
			{
				code = Encode();
				Application.ExternalCall("PushState", code);
			}
		});
	}

	void GetHash(string hash)
	{
		Decode(hash);
	}

	public bool DoEncode = false;
	public bool DoDecode = false;
	public string code;

	void Update()
	{
		if (DoEncode)
		{
			DoEncode = false;
			code = Encode();
			Debug.Log(code);
		}

		if (DoDecode)
		{
			DoDecode = false;
			Decode(code);
		}
	}

	private void Decode(string hash)
	{
		sendCallback = false;

		var values = hash.Split(',');
		uint bits, bit;

		for (int i = 0, index; i < values.Length; i++)
		{
			if (!UInt32.TryParse(values[i], out bits))
				continue;

			for (int j = 0; j < 32; j++)
			{
				bit = bits & bitMask;
				bits = bits >> 1;

				// i=0, j=0 => index = 31
				// i=0, j=31 => index = 0
				// i=1, j=0 => index = 63
				index = i * 32 + (32 - j - 1);

				toggles[index].isOn = bit > 0;
			}
		}

		sendCallback = true;
	}

	private string Encode()
	{
		List<uint> list = new List<uint>();
		for (int i = 0, index; i < length; i++)
		{
			index = i / 32;
			if (i % 32 == 0)
				list.Add(0);

			list[index] = list[index] << 1;
			list[index] = list[index] | (uint)(toggles[i].isOn ? 1 : 0);
		}
		return string.Join(",", list.ConvertAll(d => d.ToString()).ToArray());
	}
}