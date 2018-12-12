import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField,
  PropertyPaneSlider
} from '@microsoft/sp-webpart-base';

import * as strings from 'RecentFilesWebPartStrings';
import RecentFiles from './components/RecentFiles';
import { IRecentFilesProps } from './components/IRecentFilesProps';

import { MSGraphClient } from '@microsoft/sp-http';

export interface IRecentFilesWebPartProps {
  title: string;
  numberOfItemsToShow: number;
}

export default class RecentFilesWebPart extends BaseClientSideWebPart<IRecentFilesWebPartProps> {

  private graphClient: MSGraphClient;
  
  public onInit(): Promise<void> {
    this.properties.numberOfItemsToShow = 5;
    return new Promise<void>((resolve: () => void, reject: (error: any) => void): void => {
      this.context.msGraphClientFactory
        .getClient()
        .then((client: MSGraphClient): void => {
          this.graphClient = client;
          resolve();
        }, err => reject(err));
    });
  }

  public updateTitle = (value: string): void => {
    // store the new title in the title web part property
    this.properties.title = value;
  }

  public render(): void {
    const element: React.ReactElement<IRecentFilesProps > = React.createElement(
      RecentFiles,
      {
        graphClient: this.graphClient,
        updateProperty: this.updateTitle,
        title: this.properties.title,
        numberOfItemsToShow: this.properties.numberOfItemsToShow,
        displayMode: this.displayMode
      }
    );

    ReactDom.render(element, this.domElement);
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
                PropertyPaneTextField('title', {
                  label: strings.TitleFieldLabel
                }),
                PropertyPaneSlider('numberOfItemsToShow', {
                  label: strings.NumberOfItemsToShowFieldLabel,
                  min: 1,
                  max: 15,
                  step: 1
                })
              ]
            }
          ]
        }
      ]
    };
  }
}
