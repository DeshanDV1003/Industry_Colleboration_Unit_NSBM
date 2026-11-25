export type SprintStatus = 'ToDo' | 'InProgress' | 'InReview' | 'Done'

/** Mirrors Features/TeamSimulation/Dtos/SprintItemDto. */
export interface SprintItem {
  id: number
  key: string
  title: string
  description: string
  storyPoints: number
  roleName: string
  owner: string
  status: SprintStatus
  isMine: boolean
  canPickUp: boolean
}

/** Mirrors Features/TeamSimulation/Dtos/SprintBoardDto. */
export interface SprintBoard {
  myRoleName: string | null
  items: SprintItem[]
}
