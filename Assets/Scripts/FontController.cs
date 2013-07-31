using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FontController : MonoBehaviour {
	
	public GameObject[] fontAssets;
	
	private Transform thisTransform;
	private Dictionary< char , GameObject > fontDictionary = new Dictionary< char , GameObject >();
	
	void Awake () {
		thisTransform = transform;
		fontDictionary.Add( '0' , fontAssets[ 0 ] );
		fontDictionary.Add( '1' , fontAssets[ 1 ] );
		fontDictionary.Add( '2' , fontAssets[ 2 ] );
		fontDictionary.Add( '3' , fontAssets[ 3 ] );
		fontDictionary.Add( '4' , fontAssets[ 4 ] );
		fontDictionary.Add( '5' , fontAssets[ 5 ] );
		fontDictionary.Add( '6' , fontAssets[ 6 ] );
		fontDictionary.Add( '7' , fontAssets[ 7 ] );
		fontDictionary.Add( '8' , fontAssets[ 8 ] );
		fontDictionary.Add( '9' , fontAssets[ 9 ] );
		
		fontDictionary.Add( 'A' , fontAssets[ 10 ] );
		fontDictionary.Add( 'B' , fontAssets[ 11 ] );
		fontDictionary.Add( 'C' , fontAssets[ 12 ] );
		fontDictionary.Add( 'D' , fontAssets[ 13 ] );
		fontDictionary.Add( 'E' , fontAssets[ 14 ] );
		fontDictionary.Add( 'F' , fontAssets[ 15 ] );
		fontDictionary.Add( 'G' , fontAssets[ 16 ] );
		fontDictionary.Add( 'H' , fontAssets[ 17 ] );
		fontDictionary.Add( 'I' , fontAssets[ 18 ] );
		fontDictionary.Add( 'J' , fontAssets[ 19 ] );
		fontDictionary.Add( 'K' , fontAssets[ 20 ] );
		fontDictionary.Add( 'L' , fontAssets[ 21 ] );
		fontDictionary.Add( 'M' , fontAssets[ 22 ] );
		fontDictionary.Add( 'N' , fontAssets[ 23 ] );
		fontDictionary.Add( 'O' , fontAssets[ 24 ] );
		fontDictionary.Add( 'P' , fontAssets[ 25 ] );
		fontDictionary.Add( 'Q' , fontAssets[ 26 ] );
		fontDictionary.Add( 'R' , fontAssets[ 27 ] );
		fontDictionary.Add( 'S' , fontAssets[ 28 ] );
		fontDictionary.Add( 'T' , fontAssets[ 29 ] );
		fontDictionary.Add( 'U' , fontAssets[ 30 ] );
		fontDictionary.Add( 'V' , fontAssets[ 31 ] );
		fontDictionary.Add( 'W' , fontAssets[ 32 ] );
		fontDictionary.Add( 'X' , fontAssets[ 33 ] );
		fontDictionary.Add( 'Y' , fontAssets[ 34 ] );
		fontDictionary.Add( 'Z' , fontAssets[ 35 ] );
		fontDictionary.Add( '.' , fontAssets[ 36 ] );
	}

	public string text;
	
	// Use this for initialization
	void Start () {
		StringToMesh( text );
	}
	
	public void SetText ( string newText ) {
		if ( newText != null ) text = newText;
	}
	
	public int numberOfDecimals = 1;
	
	private float bufferedNumber;
	
	public void SetText ( float numberToAdd , bool replaceExisting ) {
		if ( numberToAdd != float.NaN ) {
			if ( replaceExisting ) bufferedNumber = numberToAdd;
			else {
				bufferedNumber = float.Parse( text );
				bufferedNumber += numberToAdd;
			}
			text = bufferedNumber.ToString("f" + numberOfDecimals);
		}
	}
	
	public float textAngle;
	public GameObject particles;
	public bool isMasterText;
	public bool isMirrored;
	public float fontSize;
	public float letterSpacing;
	public float punctuationSpacing;
	public string textAlignment;
	
	private string bufferedText;
	private float bufferedSize;
	private float bufferedLetterSpacing;
	private float bufferedPunctuationSpacing;
	private string bufferedAlignment;
	private char[] textArray;
	private int textLength;
	private GameObject[] letters;
	private Vector3[] charPositions;
	private char prevChar;
	private Quaternion letterRotation;
	private GameObject particleEffect;
	
	private void StringToMesh ( string inputText ) {
		bufferedText = inputText;
		bufferedSize = fontSize;
		bufferedLetterSpacing = letterSpacing;
		bufferedPunctuationSpacing = punctuationSpacing;
		bufferedAlignment = textAlignment;
		textArray = new char[ text.Length ];
		letters = new GameObject[ textArray.Length ];
		charPositions = new Vector3[ textArray.Length ];
		textArray = text.ToCharArray();
		prevChar = ' ';
		transform.rotation = Quaternion.identity;
		for ( int i = 0 ; i < textArray.Length ; i++ ) {
			if ( textAlignment == "Left" ) {
				if ( i > 0 ) {
					if ( textArray[ i ] == '.' || prevChar == '.' ) charPositions[ i ] = charPositions[ i - 1 ] + new Vector3( punctuationSpacing , 0 , 0 );
					else charPositions[ i ] = charPositions[ i - 1 ] + new Vector3( letterSpacing , 0 , 0 );
				} else charPositions[ i ] = thisTransform.position + ( new Vector3( fontSize , 0 , 0 ) / 2 );
				if ( isMirrored ) {
					letterRotation = Quaternion.AngleAxis( 0 , Vector3.right );
					letters[ i ] = Instantiate( fontDictionary[ textArray[ textArray.Length - ( i + 1 ) ] ] , charPositions[ i ] , letterRotation ) as GameObject;
					if ( particles != null ) {
						particleEffect = Instantiate( particles , charPositions[ i ] + ( new Vector3( 0 , 1 , 0 ) * ( fontSize / 2 ) ) , Quaternion.identity ) as GameObject;
						particleEffect.transform.parent = thisTransform;
					}
					prevChar = textArray[ textArray.Length - ( i + 1 ) ];
				}
				else {
					letterRotation = Quaternion.LookRotation( Vector3.back );
					letters[ i ] = Instantiate( fontDictionary[ textArray[ i ] ] , charPositions[ i ] , letterRotation ) as GameObject;
					if ( particles != null ) {
						particleEffect = Instantiate( particles , charPositions[ i ] + ( new Vector3( 0 , 1 , 0 ) * ( fontSize / 2 ) ) , Quaternion.identity ) as GameObject;
						particleEffect.transform.parent = thisTransform;
					}
					prevChar = textArray[ i ];
				}

			}
			else if ( textAlignment == "Right" ) {
				if ( i > 0 ) {
					if ( textArray[ textArray.Length - ( i + 1 ) ] == '.' || prevChar == '.' ) charPositions [ i ] = charPositions[ i - 1 ] + new Vector3( -punctuationSpacing , 0 , 0 );
					else charPositions[ i ] = charPositions[ i - 1 ] + new Vector3( -letterSpacing , 0 , 0 );
				} else charPositions[ i ] = thisTransform.position - ( new Vector3( fontSize , 0 , 0 ) / 2 ) ;
				if ( isMirrored ) {
					letterRotation = Quaternion.AngleAxis( 0 , Vector3.right );
					letters[ i ] = Instantiate( fontDictionary[ textArray[ i ] ] , charPositions[ i ] , letterRotation ) as GameObject;
					if ( particles != null ) {
						particleEffect = Instantiate( particles , charPositions[ i ] + ( new Vector3( 0 , 1 , 0 ) * ( fontSize / 2 ) ) , Quaternion.identity ) as GameObject;
						particleEffect.transform.parent = thisTransform;
					}
					prevChar = textArray[ textArray.Length - ( i + 1 ) ];
				}
				else {
					letterRotation = Quaternion.LookRotation( Vector3.back );
					letters[ i ] = Instantiate( fontDictionary[ textArray[ textArray.Length - ( i + 1 ) ] ] , charPositions[ i ] , letterRotation ) as GameObject;
					if ( particles != null ) {
						particleEffect = Instantiate( particles , charPositions[ i ] + ( new Vector3( 0 , 1 , 0 ) * ( fontSize / 2 ) ) , Quaternion.identity ) as GameObject;
						particleEffect.transform.parent = thisTransform;
					}
					prevChar = textArray[ textArray.Length - ( i + 1 ) ];
				}
			}
			letters[ i ].transform.localScale = Vector3.one * fontSize;
			letters[ i ].transform.parent = thisTransform;
			letters[ i ].transform.name = textArray[ i ].ToString();
		}
		//if ( isMasterText ) thisTransform.parent.SendMessage( "SetRotation" );
		transform.rotation = Quaternion.AngleAxis( textAngle , Vector3.forward );

	}
	
	GameObject bufferedLetter;
	
	// Update is called once per frame
	void Update () {
		if ( text != bufferedText || fontSize != bufferedSize || bufferedLetterSpacing != letterSpacing || bufferedPunctuationSpacing != punctuationSpacing || bufferedAlignment != textAlignment ) {
			foreach ( GameObject letter in letters ) {
				bufferedLetter = letter;
				Destroy( bufferedLetter );
			}
			StringToMesh( text );
		}
	}
}
