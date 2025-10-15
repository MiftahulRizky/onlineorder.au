<%@ Page Title="Hardware Kit" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Kit_Default" MaintainScrollPositionOnPostback="true" Debug="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Hadware Kit</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="card col-12" id="card-table">
                    <div class="card-header row">
                        <div class="col-lg-5">
                            <h3 class="card-title">Data Hadware Kit</h3>
                        </div>
                        <div class="col-lg-2">
                            <select name="designid" id="designid" class="form-select"></select>
                        </div>
                        <div class="col-lg-2">
                            <select name="blindid" id="blindid" class="form-select"></select>
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-primary w-100" id="btn-add">
                                <i class="ti ti-plus opacity-50 fs-2 me-1"></i>
                                Create New
                            </button>
                        </div>
                        <div class="col-lg-1">
                            <div class="dropdown ">
                                <button class="btn btn-secondary dropdown-toggle w-100" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    <i class="ti ti-list-details fs-2 opacity-50 me-1"></i>
                                    Kits
                                </button>
                                <ul class="dropdown-menu dropdown-menu-end dropdown-menu-arrow">
                                    <span class="dropdown-header">additional kits</span>
                                    <li><a class="dropdown-item" href="/setting/kit/bracket">Bracket</a></li>
                                    <li><a class="dropdown-item" href="/setting/kit/tube">Tube</a></li>
                                    <li><a class="dropdown-item" href="/setting/kit/control">Control</a></li>
                                    <li><a class="dropdown-item" href="/setting/kit/colour">Colour</a></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="card-body" id="table-default">
                        <table class="table table-hover table-vcenter card-table w-100" id="data-table">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>soe id</th>
                                    <th>kit name</th>
                                    <th>design name</th>
                                    <th>blind name</th>
                                    <th>action</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <!-- Modal -->
    <div class="modal fade" id="modalSubmit" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalSubmitLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content ">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalSubmitLabel">Modal title</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <form action="javascript:void(0)" method="post" id="form-submit">
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-lg-2 mb-3">
                                <label for="id" class="form-label text-uppercase">soe id</label>
                                <input type="number" name="soeid" id="soeid" class="form-control">
                                <input type="text" name="id" id="id" class="form-control" readonly hidden>
                            </div>
                            <div class="col-lg-10 mb-3">
                                <label for="name" class="form-label text-uppercase">kit name</label>
                                <input type="text" name="name" id="name" class="form-control">
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="designtype" class="form-label text-uppercase">design type</label>
                                <select name="designtype" id="designtype" class="form-select"></select>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="blindtype" class="form-label text-uppercase">blind type</label>
                                <select name="blindtype" id="blindtype" class="form-select"></select>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="bracket" class="form-label text-uppercase">bracket type</label>
                                <select name="bracket" id="bracket" class="form-select"></select>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="tube" class="form-label text-uppercase">tube type</label>
                                <select name="tube" id="tube" class="form-select"></select>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="control" class="form-label text-uppercase">control type</label>
                                <select name="control" id="control" class="form-select"></select>
                            </div>
                            <div class="col-lg-4 mb-3">
                                <label for="colour" class="form-label text-uppercase">colour type</label>
                                <select name="colour" id="colour" class="form-select"></select>
                            </div>
                            <div class="col-lg-10 mb-3">
                                <label for="des" class="form-label text-uppercase">description</label>
                                <textarea name="des" id="des" class="form-control" cols="1" rows="1"></textarea>
                            </div>
                            <div class="col-lg-2 mb-3">
                                <label for="active" class="form-label text-uppercase">active</label>
                                <select name="active" id="active" class="form-select">
                                    <option value="1">YES</option>
                                    <option value="0">NO</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                        <button type="submit" class="btn btn-primary" id="btn-submit">Submit</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        let userId = '<%= Session("userId") %>';
        let userName = '<%= Session("UserName") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';  
        let storeId = '<%= Session("StoreId") %>';
        let storeCompany = '<%= Session("StoreCompany") %>';
        let levelName = '<%= Session("LevelName") %>';
        let uriMethod = '/Methods/Setting/Kit/KitMethod.aspx'; 

        
    </script>
    <script src="/Scripts/Setting/Kit/Default.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>

