import { IFilePickerSelectedFile } from "../FilePickerWebPart";

export interface IFilePickerProps {
    selectedFiles: IFilePickerSelectedFile[];
    onConfigure: () => void;
}
