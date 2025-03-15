using System;
using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>An 8x8 matrix representation of a chessboard.</summary>
    public class Board
    {
        private readonly Piece[,] boardMatrix;
        private readonly Dictionary<Side, Square?> currentKingSquareBySide = new Dictionary<Side, Square?>
        {
            [Side.White] = null,
            [Side.Black] = null
        };

        /// <summary>
        /// Checks if a move obeys the chess rules.
        /// </summary>
        public bool MoveObeysRules(Movement move, Side playerSide)
        {
            // Check if the move is within valid chess rules
            if (move == null || move.Start == move.End)
            {
                return false; // Invalid move
            }

            // Ensure the piece being moved belongs to the correct player
            Piece movingPiece = this[move.Start];
            if (movingPiece == null || movingPiece.Owner != playerSide)
            {
                return false; // Not the player's piece
            }

            // Ensure the move is in the piece's valid moveset
            Dictionary<(Square, Square), Movement> legalMoves = movingPiece.CalculateLegalMoves(this, new GameConditions(), move.Start);
            return legalMoves.ContainsKey((move.Start, move.End));
        }


        public bool IsPlayerCheckmated(Side playerSide)
        {
            if (!IsPlayerInCheck(playerSide)) return false; // Not checkmated if not in check

            // Loop through all the player's pieces and check if they have any legal moves
            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Piece piece = this[file, rank];
                    if (piece != null && piece.Owner == playerSide)
                    {
                        if (HasLegalMoves(piece)) return false; // If any piece has a move, not checkmate
                    }
                }
            }

            return true; // No legal moves & in check → Checkmate
        }

        public bool IsPlayerStalemated(Side playerSide)
        {
            if (IsPlayerInCheck(playerSide)) return false; // Not stalemate if in check

            // Loop through all the player's pieces and check if they have any legal moves
            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Piece piece = this[file, rank];
                    if (piece != null && piece.Owner == playerSide)
                    {
                        if (HasLegalMoves(piece)) return false; // If any piece has a move, not stalemate
                    }
                }
            }

            return true; // No legal moves & NOT in check → Stalemate
        }

        private bool HasLegalMoves(Piece piece)
        {
            return piece.GetValidMoves(this).Count > 0;
        }

        public bool IsPlayerInCheck(Side playerSide)
        {
            Square kingSquare = GetKingSquare(playerSide);
            if (!kingSquare.IsValid()) return false; // No valid king position found

            return IsSquareAttacked(kingSquare, playerSide == Side.White ? Side.Black : Side.White);
        }

        public bool IsSquareAttacked(Square position, Side attackerSide)
        {
            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Piece piece = this[file, rank];
                    if (piece != null && piece.Owner == attackerSide)
                    {
                        if (piece.CanAttack(position, this)) return true;
                    }
                }
            }
            return false;
        }

        public bool IsOccupiedAt(Square position) => this[position] != null;

        public bool IsOccupiedBySideAt(Square position, Side side) => this[position] is Piece piece && piece.Owner == side;

        public void MovePiece(Movement move)
        {
            if (this[move.Start] is not { } pieceToMove)
            {
                throw new ArgumentException($"No piece was found at the given position: {move.Start}");
            }

            this[move.Start] = null;
            this[move.End] = pieceToMove;

            if (pieceToMove is King)
            {
                currentKingSquareBySide[pieceToMove.Owner] = move.End;
            }

            (move as SpecialMove)?.HandleAssociatedPiece(this);
        }

        public Piece this[Square position]
        {
            get
            {
                if (position.IsValid()) return boardMatrix[position.File - 1, position.Rank - 1];
                throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
            }

            set
            {
                if (position.IsValid()) boardMatrix[position.File - 1, position.Rank - 1] = value;
                else throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
            }
        }

        public Piece this[int file, int rank]
        {
            get => this[new Square(file, rank)];
            set => this[new Square(file, rank)] = value;
        }

        public Board(params (Square, Piece)[] squarePiecePairs)
        {
            boardMatrix = new Piece[8, 8];

            foreach ((Square position, Piece piece) in squarePiecePairs)
            {
                this[position] = piece;
            }
        }

        public Board(Board board)
        {
            boardMatrix = new Piece[8, 8];
            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    Piece pieceToCopy = board[file, rank];
                    if (pieceToCopy == null) { continue; }

                    this[file, rank] = pieceToCopy.DeepCopy();
                }
            }
        }

        public void ClearBoard()
        {
            for (int file = 1; file <= 8; file++)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    this[file, rank] = null;
                }
            }

            currentKingSquareBySide[Side.White] = null;
            currentKingSquareBySide[Side.Black] = null;
        }

        public static readonly (Square, Piece)[] StartingPositionPieces = {
            (new Square("a1"), new Rook(Side.White)),
            (new Square("b1"), new Knight(Side.White)),
            (new Square("c1"), new Bishop(Side.White)),
            (new Square("d1"), new Queen(Side.White)),
            (new Square("e1"), new King(Side.White)),
            (new Square("f1"), new Bishop(Side.White)),
            (new Square("g1"), new Knight(Side.White)),
            (new Square("h1"), new Rook(Side.White)),

            (new Square("a2"), new Pawn(Side.White)),
            (new Square("b2"), new Pawn(Side.White)),
            (new Square("c2"), new Pawn(Side.White)),
            (new Square("d2"), new Pawn(Side.White)),
            (new Square("e2"), new Pawn(Side.White)),
            (new Square("f2"), new Pawn(Side.White)),
            (new Square("g2"), new Pawn(Side.White)),
            (new Square("h2"), new Pawn(Side.White)),

            (new Square("a8"), new Rook(Side.Black)),
            (new Square("b8"), new Knight(Side.Black)),
            (new Square("c8"), new Bishop(Side.Black)),
            (new Square("d8"), new Queen(Side.Black)),
            (new Square("e8"), new King(Side.Black)),
            (new Square("f8"), new Bishop(Side.Black)),
            (new Square("g8"), new Knight(Side.Black)),
            (new Square("h8"), new Rook(Side.Black)),

            (new Square("a7"), new Pawn(Side.Black)),
            (new Square("b7"), new Pawn(Side.Black)),
            (new Square("c7"), new Pawn(Side.Black)),
            (new Square("d7"), new Pawn(Side.Black)),
            (new Square("e7"), new Pawn(Side.Black)),
            (new Square("f7"), new Pawn(Side.Black)),
            (new Square("g7"), new Pawn(Side.Black)),
            (new Square("h7"), new Pawn(Side.Black)),
        };

        public Square GetKingSquare(Side player)
        {
            if (currentKingSquareBySide[player] == null)
            {
                for (int file = 1; file <= 8; file++)
                {
                    for (int rank = 1; rank <= 8; rank++)
                    {
                        if (this[file, rank] is King king)
                        {
                            currentKingSquareBySide[king.Owner] = new Square(file, rank);
                        }
                    }
                }
            }

            return currentKingSquareBySide[player] ?? Square.Invalid;
        }
    }
}
