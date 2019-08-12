import { AdminService } from './../../_services/admin.service';
import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-photo-management',
    templateUrl: './photo-management.component.html',
    styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
    photos: any;


    constructor(private adminService: AdminService) { }

    ngOnInit() {
        this.getPhotosForApproval();
        console.log('photos ', this.photos);
    }

    getPhotosForApproval() {
        this.adminService.getPhotosForApproval().subscribe((photos) => {
            this.photos = photos;
            console.log(this.photos);
        }, error => {
            console.log(error);
        });
    }

    approvePhoto(photoId: string) {
        this.adminService.approvePhoto(photoId).subscribe(_ => {
            this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
        }, error => {
            console.log(error);
        });
    }

    rejectphoto(photoId: string) {
        return this.adminService.rejectPhoto(photoId).subscribe(() => {
            this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1);
        }, error => {
            console.log(error);
        });
    }
}
