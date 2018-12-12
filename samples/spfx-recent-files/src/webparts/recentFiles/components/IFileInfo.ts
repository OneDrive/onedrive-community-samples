export interface IItemInfo {
  //'@odata.etag': string;
  id: string;
  name: string;
  file: IFileProperty;
  webUrl: string;
  lastModifiedBy: IModifiedBy;
}

export interface IItemInfos {
  value: IItemInfo[];
}

export interface IFileProperty {
  mimeType: string;
}

export interface IModifiedBy {
  user: IUserProperty;
}

export interface IUserProperty {
  displayName: string;
}
