import { AlertifyService } from './../../_services/alertify.service';
import { AuthService } from './../../_services/auth.service';
import { environment } from 'src/environments/environment';
import { Photo } from './../../_models/photo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemeberPhotoChange = new EventEmitter<string>();

  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(private authService: AuthService,
              private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const result: Photo = JSON.parse(response);
        const photo = {
          id: result.id,
          url: result.url,
          dateAdded: result.dateAdded,
          description: result.description,
          isMain: result.isMain
        };

        this.photos.push(photo);

        if (photo.isMain) {
          this.updateMemberPhoto(photo.url);
        }
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMain = this.photos.filter(p => p.isMain)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.updateMemberPhoto(photo.url);
      this.alertify.success('Successfully set to main');
    }, error => {
      this.alertify.error(error);
    });
  }

  deletePhoto(id: string) {
    this.alertify.confirm('Delete Photo', 'Are you sure you want to delete this photo?', () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertify.success('Photo has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the photo');
      });
    }, () => { } );
  }

  updateMemberPhoto(photoUrl: string) {
    this.authService.changeMemberPhoto(photoUrl);
    this.authService.currentUser.photoUrl = photoUrl;
    localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
  }
}
