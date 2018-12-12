import * as React from 'react';
import styles from './RecentFiles.module.scss';
import { IRecentFilesProps } from './IRecentFilesProps';
import { IItemInfo, IItemInfos } from './IFileInfo';
import { DisplayMode } from '@microsoft/sp-core-library';
import { WebPartTitle } from '@pnp/spfx-controls-react/lib/WebPartTitle';
import { ListView, IViewField, SelectionMode, GroupOrder, IGrouping } from "@pnp/spfx-controls-react/lib/ListView";
import { Spinner, SpinnerSize } from 'office-ui-fabric-react/lib/components/Spinner';
import { IRecentFilesState } from './IRecentFilesState';

// Set the fields to show in the ListView element
const viewFields: IViewField[] = [
  {
    name: 'name',
    displayName: 'Name',
    minWidth: 100
  },
  {
    name: 'lastModifiedDateTime',
    displayName: 'Modified date',
    minWidth: 100
  },
  {
    name: 'lastModifiedBy.user.displayName',
    displayName: 'Modified by',
    minWidth: 100
  },
  {
    name: '',
    minWidth: 100,
    render: (item, index, column) => { return (< a href={ item.webUrl } target='_blank'>Open</a>); }
  }
];

export default class RecentFiles extends React.Component<IRecentFilesProps, IRecentFilesState> {

  constructor(props: IRecentFilesProps) {
    super(props);
    
    // Initialize the state
    this.state = {
      items: [],
      loading: false,
      error: undefined
    };
  }

  private _loadFiles(): void {

    // Start the spinner
    this.setState({
      loading: true,
    });

    // Call the Graph API
    this.props.graphClient
      .api('me/drive/recent')
      .version('v1.0')
      .select('id,name,webUrl,lastModifiedDateTime,lastModifiedBy')
      .top((this.props.numberOfItemsToShow !== undefined) ? this.props.numberOfItemsToShow : 5)
      .orderby('name desc')
      .get((err: any, res: IItemInfos): void => {
        // If err is specified
        if (err) {
          // Something failed calling the MS Graph API
          this.setState({
            error: err.message || 'There was an error while calling Microsoft Graph API.',
            loading: false
          });
          return;
        }

        // Check if a response was retrieved
        if (res && res.value && res.value.length > 0) {
          // If there is a response set the state
          this.setState({
            items: res.value,
            loading: false
          });
        }
        else {
          // If no file found
          this.setState({
            loading: false
          });
        }
      });
  }

  public componentDidMount(): void {
    // load data initially after the component has been instantiated
    this._loadFiles();
  }

  public componentDidUpdate(prevProps: IRecentFilesProps, prevState: IRecentFilesState): void {
    // verify if the component should update. Helps avoid unnecessary re-renders
    // when the parent has changed but this component hasn't
    if (prevProps.numberOfItemsToShow !== this.props.numberOfItemsToShow) {
      this._loadFiles();
    }
  }

  public render(): React.ReactElement<IRecentFilesProps> {
    return (
      <div className={ styles.recentFiles }>
        <WebPartTitle title={this.props.title} 
          displayMode={ this.props.displayMode }
          updateProperty={ this.props.updateProperty }
          className={ styles.title } />
        {
          this.state.loading ?
          <Spinner label='Loading...' size={ SpinnerSize.large } /> 
          :
          this.state.items &&
            this.state.items.length > 0 ? (
              <ListView
                items={this.state.items}
                viewFields={viewFields}
                compact={false}
                iconFieldName='webUrl'
                selectionMode={SelectionMode.single} />
            ) : (
              !this.state.loading && (
                this.state.error ?
                  <span className={ styles.error }>{ this.state.error }</span> :
                  <span className={ styles.noFiles }>No files found.</span>
              )
            )
        }
      </div>
    );

  }
}
