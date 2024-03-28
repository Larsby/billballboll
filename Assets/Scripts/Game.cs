using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
class Game
{ 
	public enum Piece { Empty = 0, X = 1, O = 2 };

	static int[,] winConditions = new int[8, 3]
	{
		{ 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 },
		{ 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 },
		{ 0, 4, 8 }, { 2, 4, 6 }
	};

	public Piece[] Grid = new Piece[9];

	public Piece CurrentTurn = Piece.X;

	int Choice = 0;

	public Piece Computer;
	public Piece Player;

	public Game()
	{
		CurrentTurn = Piece.X;
		Player = Piece.X;
	}

	public void Reset()
	{
		CurrentTurn = Piece.X;
		SetPlayer(Piece.X);
		Grid = new Piece[9];
	}

	public void SetPlayer(Piece Player)
	{
		this.Player = Player;
		this.Computer = switchPiece(Player);
	}

	public void MakeMove(int Move)
	{
		if(CurrentTurn == Player)
		{
			Grid = makeGridMove(Grid, CurrentTurn, Move);
			CurrentTurn = switchPiece(CurrentTurn);
		}
		else if(CurrentTurn == Computer)
		{
			minimax(cloneGrid(Grid), CurrentTurn);
			Grid = makeGridMove(Grid, CurrentTurn, Choice);
			CurrentTurn = switchPiece(CurrentTurn);
			//Debug.Log(Choice.ToString());
		}
	}

	int minimax(Piece[] InputGrid, Piece Player)
	{
		Piece[] Grid = cloneGrid(InputGrid);

		if (checkScore(Grid, Player) != 0)
			return checkScore(Grid, Player);
		else if (checkGameEnd(Grid)) return 0;

		List<int> scores = new List<int>();
		List<int> moves = new List<int>();

		for (int i = 0; i < 9; i++)
		{
			if (Grid[i] == Piece.Empty)
			{
				scores.Add(minimax(makeGridMove(Grid, Player, i), switchPiece(Player)));
				moves.Add(i);
			}
		}

		if(Player == Computer)
		{
			int MaxScoreIndex = scores.IndexOf(scores.Max());
			Choice = moves[MaxScoreIndex];
			return scores.Max();
		}
		else
		{
			int MinScoreIndex = scores.IndexOf(scores.Min());
			Choice = moves[MinScoreIndex];
			return scores.Min();
		}
	}

	static int checkScore(Piece[] Grid, Piece Player)
	{
		if (checkGameWin(Grid, Player)) return 10;

		else if (checkGameWin(Grid, switchPiece(Player))) return -10;

		else return 0;
	}

	static bool checkGameWin(Piece[] Grid, Piece Player)
	{
		for(int i = 0; i < 8; i++)
		{
			if
				(
					Grid[winConditions[i, 0]] == Player &&
					Grid[winConditions[i, 1]] == Player &&
					Grid[winConditions[i, 2]] == Player
				)
			{
				return true;
			}
		}
		return false;
	}

	static bool checkGameEnd(Piece[] Grid)
	{
		foreach (Piece p in Grid) if (p == Piece.Empty) return false;
		return true;
	}

	static Piece switchPiece(Piece Piece)
	{
		if (Piece == Piece.X) return Piece.O;
		else return Piece.X;
	}

	static Piece[] cloneGrid(Piece[] Grid)
	{
		Piece[] Clone = new Piece[9];
		for (int i = 0; i < 9; i++) Clone[i] = Grid[i];

		return Clone;
	}

	static Piece[] makeGridMove(Piece[] Grid, Piece Move, int Position)
	{
		Piece[] newGrid = cloneGrid(Grid);
		newGrid[Position] = Move;
		return newGrid;
	}
}
