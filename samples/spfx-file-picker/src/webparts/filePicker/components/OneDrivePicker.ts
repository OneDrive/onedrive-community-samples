var OneDrive : any = require('OneDriveExternal');

export interface OneDriveThumbnailItem {
    url: string;
}

export interface OneDriveThumbnail {
    id: string;
    small: OneDriveThumbnailItem;
    medium: OneDriveThumbnailItem;
    large: OneDriveThumbnailItem;
}

export interface OneDriveItem {
    id: string;
    name: string;
    size: number;
    webUrl: string;
    thumbnails: OneDriveThumbnail[];
}

export interface OneDrivePickerResult {
    value: OneDriveItem[];
    accessToken: string;
    apiEndpoint: string;
}

export interface OneDriveAdvancedOptions {
    filter?: string;
    queryParameters?: string;
}

export interface OneDriveOptions {
    clientId: string;
    action: string;
    multiSelect: boolean;
    advanced: OneDriveAdvancedOptions;
    success: (r: OneDrivePickerResult) => void;
    cancel: () => void;
    error: (e: Error) => void;
}

export class OneDrivePicker {
    public open(success: (r: OneDrivePickerResult) => void, cancel?: () => void, error?: (e: Error) => void) {        
        var odOptions: OneDriveOptions = {
            clientId: "[YOUR APP CLIENTID HERE]",
            action: "query",
            multiSelect: true,
            advanced: {
                filter: ".png,.jpg,.jpeg",
                queryParameters: "select=id,name,size,file,folder,photo,@microsoft.graph.downloadUrl&expand=thumbnails"
            },
            success: success,
            cancel: cancel,
            error: error
        };

        OneDrive.open(odOptions);
    }
}