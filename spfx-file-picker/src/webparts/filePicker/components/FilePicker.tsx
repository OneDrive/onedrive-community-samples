import * as React from 'react';
import styles from './FilePicker.module.scss';
import { IFilePickerProps } from './IFilePickerProps';
import { Placeholder } from "@pnp/spfx-controls-react/lib/Placeholder";

export default class FilePicker extends React.Component<IFilePickerProps, {}> {

  public render(): React.ReactElement<IFilePickerProps> {
    const needsConfiguration = (this.props.selectedFiles == null || this.props.selectedFiles.length <= 0);

    return (
      <div className={ styles.filePicker }>
        { needsConfiguration  &&
          <Placeholder
            iconName='Edit'
            iconText='Configure your web part'
            description='Please configure the web part.'
            buttonLabel='Configure'
            onConfigure={this.props.onConfigure} />}
        {!needsConfiguration &&
          <div className={ styles.container }>
            <div>
              {
                (this.props.selectedFiles != undefined) ?
                // For every selected image create an img tag in the output
                this.props.selectedFiles.map(image => <img src={ image.thumbnailUrl } className={ styles.oneDriveItem }></img>)
                : undefined
              }
            </div>
          </div>}
      </div>
    );
  }
}
