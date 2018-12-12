import { MSGraphClient } from "@microsoft/sp-http";
import { IRecentFilesWebPartProps } from "../RecentFilesWebPart";
import { DisplayMode } from "@microsoft/sp-core-library";

export interface IRecentFilesProps extends IRecentFilesWebPartProps {
  graphClient: MSGraphClient;
  updateProperty: (value: string) => void;
  displayMode: DisplayMode;
}
