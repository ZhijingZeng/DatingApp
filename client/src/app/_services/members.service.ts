import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';
//services are singltons, can persist the data
@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl
  members: Member[] = [];
  constructor(private http: HttpClient) { }
  getMembers() {
    if (this.members.length > 0) return of(this.members)
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      map(members => {
        this.members = members;

        return members;
      })
    )
  }

  getMember(username: string) {
    const member = this.members.find(x => x.userName == username);
    if (member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member) {

    return this.http.put(this.baseUrl + 'users', member)
    // .pipe(
    //   map(_ => {
    //     const index = this.members.indexOf(member);
    //     console.log("member",member);
    //     console.log("members[4]",this.members[4]);
    //     this.members[index] = { ...this.members[index], ...member }//deep copy
    //   })
    // )
    
  }
  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId,{});
  }

  deletePhoto(photoId:number){
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }

  
  
}
