using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AI.NCG {

	public class DungeonGenerator : MonoBehaviour {

		public int mapSize = 11;
		public GameObject cellPrefab;

		Cell[,] grid;


		public enum Direction {

			N, E, S, W

		}



		void Start () {
		
			grid = new Cell[ mapSize, mapSize ];

			StartCoroutine ( GeneratreCo() );

		}
	
		
		public  List<Cell> GetNeigh( Cell c, int step ) {

			List<Cell> neigh = new List<Cell>();

			// N
			if ( c.Y < mapSize - step )
				neigh.Add( grid[ c.X, c.Y + step] );
			else neigh.Add( null );

			// E
			if ( c.X < mapSize - step )
				neigh.Add( grid[ c.X + step, c.Y ] );
			else neigh.Add( null );

			// S
			if ( c.Y > step - 1 )
				neigh.Add( grid[c.X, c.Y - step ] );
			else neigh.Add( null );

			// W
			if ( c.X > step - 1 )
				neigh.Add( grid[c.X - step, c.Y ] );
			else neigh.Add( null );

			return neigh;

		}


		public Cell GetNieigh( Cell c, Direction d ) {

			return grid [ c.X  - ( ( int )d - 2  ) % 2,    c.Y  - ( ( int )d - 1  ) % 2 ];

		}

		



		IEnumerator GeneratreCo(  ) {


			// create a full dungeon

			GameObject container = new GameObject( "NodeContainer" );

			for ( int x = 0; x < mapSize; x++ ) {

				for ( int y = 0; y < mapSize; y++ ) {

					var cellGO = Instantiate( cellPrefab, container.transform );

					cellGO.transform.position = new Vector3( x, y, 0f );

					var cell = cellGO.GetComponent<Cell>();

					cell.X = x;
					cell.Y = y;

					grid[ x,y ] = cell;
				}

			}

			// choose start cell

			int startX = Random.Range( 0, mapSize/2 )*2;
			int startY = Random.Range( 0, mapSize/2 )*2;

			grid[ startX, startY ].spriteRenderer.color = Color.blue;

			grid[ startX, startY ].visited = true;
			Cell currentCell = grid[ startX, startY ];

			Stack<Cell> backtrackingCells = new Stack<Cell>();
			backtrackingCells.Push( currentCell );

			while( true ) {


				// get room cells
				var neighs = GetNeigh( currentCell, 2 );


				// keep unvisited cells
				var unvisitedNeighs = neighs.Where( c => c != null && !c.visited ).ToList();


				if ( unvisitedNeighs.Count == 0 ) {

					// TODO : back tracking
					currentCell = backtrackingCells.Pop();


				}
				else {

					// choose a random unvisited neigh
					Cell rndNeigh;
					int rndIndex;
					do {

						rndIndex = Random.Range( 0, neighs.Count );
						rndNeigh = neighs [ rndIndex ];

					} while ( !unvisitedNeighs.Contains( rndNeigh ) );

					Direction rndDir = ( Direction ) rndIndex;

					rndNeigh.spriteRenderer.color = Color.black;
					rndNeigh.visited = true;

					Cell wallNeigh = GetNieigh( currentCell, rndDir );
					wallNeigh.spriteRenderer.color = Color.black;
					wallNeigh.visited = true;

					// move to next cell
					currentCell = rndNeigh;
					backtrackingCells.Push( currentCell );


				}


				if ( backtrackingCells.Count == 0 )
					break;

				yield return null;


			}





			yield return null;

		}



	}


}