import { IItemInfo } from "./IFileInfo";

export interface IRecentFilesState {
  loading: boolean;
  items: IItemInfo[];
  error: any;
}
