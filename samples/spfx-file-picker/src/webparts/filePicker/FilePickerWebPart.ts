import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField,
  PropertyPaneButton,
  PropertyPaneButtonType
} from '@microsoft/sp-webpart-base';

import * as strings from 'FilePickerWebPartStrings';
import FilePicker from './components/FilePicker';
import { IFilePickerProps } from './components/IFilePickerProps';
import { OneDrivePicker } from './components/OneDrivePicker';
import { MSGraphClient } from '@microsoft/sp-http';

export interface IFilePickerWebPartProps {
  selectedFiles: IFilePickerSelectedFile[];
}

export interface IFilePickerSelectedFile {
  url: string;
  thumbnailUrl: any; 
}

export default class FilePickerWebPart extends BaseClientSideWebPart<IFilePickerWebPartProps> {

  // private member to hold a reference to the MSGraphClient instance
  private graphClient: MSGraphClient;
  
  public onInit(): Promise<void> {
    return new Promise<void>((resolve: () => void, reject: (error: any) => void): void => {
      // let's get a reference to the MSGraphClient instance
      this.context.msGraphClientFactory
        .getClient()
        .then((client: MSGraphClient): void => {
          this.graphClient = client;
          resolve();
        }, err => reject(err));
    });
  }

  // method to start configuration
  private _onConfigure = (): void => {
    this.context.propertyPane.open();
  }
  
  public render(): void {
    const element: React.ReactElement<IFilePickerProps> = React.createElement(
      FilePicker,
      {
        selectedFiles: this.properties.selectedFiles,
        onConfigure: this._onConfigure,
      }
    );

    ReactDom.render(element, this.domElement);
  }

  protected get disableReactivePropertyChanges(): boolean {
    return true;
  }

  // function to select image files from OneDrive
  private launchOneDrivePicker = () => {

    // open the OneDrive picker window
    new OneDrivePicker().open(
      r => {
        // in case of success and if there are selected items
        if(r && r.value.length > 0) {
          let temp: IFilePickerSelectedFile[] = [];

          // cycle through the items
          r.value.forEach(item => {
            if(item.thumbnails && item.thumbnails.length > 0){
              item.thumbnails.forEach(t => {
                let i: IFilePickerSelectedFile = {
                    url: item.webUrl,
                    thumbnailUrl: t.medium.url
                };

                temp.push(i);
              });
            }
          });
          
          this.properties.selectedFiles = temp;
        }
    } , 
    () => { 
      // handle the operation cancelled event
      console.log("Operation cancelled"); 
    }, 
    e => { 
      // handle any exception
      console.log("Error: " + e); 
    }); 
  }

  // function to clean the list of selected image files
  private cleanSelectedFiles = () => {
    this.properties.selectedFiles = [];
  }

  protected onDispose(): void {
    ReactDom.unmountComponentAtNode(this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: strings.PropertyPaneDescription
          },
          groups: [
            {
              groupName: strings.BasicGroupName,
              groupFields: [
                PropertyPaneButton('openFromOneDrive', {  
                  text: "Open from OneDrive",  
                  buttonType: PropertyPaneButtonType.Primary,  
                  onClick: this.launchOneDrivePicker
                 }),
                PropertyPaneButton('cleanSelectedFiles', {  
                  text: "Clean selected files",  
                  buttonType: PropertyPaneButtonType.Normal,  
                  onClick: this.cleanSelectedFiles
                 })
              ]
            }
          ]
        }
      ]
    };
  }
}
