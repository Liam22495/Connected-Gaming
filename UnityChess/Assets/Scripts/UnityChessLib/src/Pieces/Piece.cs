using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>Base class for any chess piece.</summary>
    public abstract class Piece<T> : Piece where T : Piece<T>, new()
    {
        protected Piece() : base(Side.None) { }
        protected Piece(Side owner) : base(owner) { }

        public override Piece DeepCopy()
        {
            return new T { Owner = this.Owner };
        }
    }

    /// <summary>Base class for all chess pieces.</summary>
    public abstract class Piece
    {
        public Side Owner { get; protected set; }

        protected Piece(Side owner)
        {
            Owner = owner;
        }

        public abstract Piece DeepCopy();

        public abstract Dictionary<(Square, Square), Movement> CalculateLegalMoves(
            Board board,
            GameConditions gameConditions,
            Square position
        );

        public override string ToString() => $"{Owner} {GetType().Name}";

        public string ToTextArt() => this switch
        {
            Bishop { Owner: Side.White } => "♝",
            Bishop { Owner: Side.Black } => "♗",
            King { Owner: Side.White } => "♚",
            King { Owner: Side.Black } => "♔",
            Knight { Owner: Side.White } => "♞",
            Knight { Owner: Side.Black } => "♘",
            Queen { Owner: Side.White } => "♛",
            Queen { Owner: Side.Black } => "♕",
            Pawn { Owner: Side.White } => "♟",
            Pawn { Owner: Side.Black } => "♙",
            Rook { Owner: Side.White } => "♜",
            Rook { Owner: Side.Black } => "♖",
            _ => "?"
        };

        /// <summary>
        /// Returns a list of valid moves for this piece.
        /// </summary>
        public List<Movement> GetValidMoves(Board board)
        {
            List<Movement> validMoves = new List<Movement>();

            foreach (var move in CalculateLegalMoves(board, new GameConditions(), Square.Invalid).Values)
            {
                if (board.MoveObeysRules(move, this.Owner))
                {
                    validMoves.Add(move);
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Determines if this piece can attack a specific square.
        /// </summary>
        public bool CanAttack(Square targetSquare, Board board)
        {
            foreach (var move in CalculateLegalMoves(board, new GameConditions(), Square.Invalid).Values)
            {
                if (move.End == targetSquare)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
