using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorHandler : MonoBehaviour
{
    public Piece _currentPiece = null;
    public Piece CurrentPiece { get { return _currentPiece; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Piece piece = collision.gameObject.GetComponent<Piece>();
        if (piece != null || piece is BasePiece)
            _currentPiece = piece;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Piece piece = collision.gameObject.GetComponent<Piece>();
        if (piece != null && (_currentPiece == null || (!(_currentPiece is BasePiece) && piece is BasePiece)))
            _currentPiece = piece;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_currentPiece != null && collision.gameObject == _currentPiece.gameObject)
            _currentPiece = null;
    }
}
