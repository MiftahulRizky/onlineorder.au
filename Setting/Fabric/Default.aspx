<%@ Page Title="Fabric" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Fabric_Default" MaintainScrollPositionOnPostback="true" Debug="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Fabric</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="card col-12" id="card-table">
                    <div class="card-header row">
                        <div class="col-lg-7">
                            <h3 class="card-title">Data Fabric</h3>
                        </div>
                        <div class="col-lg-2">
                            <select name="designid" id="designid" class="form-select"></select>
                        </div>
                        <div class="col-lg-1">
                            <select name="active" id="active" class="form-select">
                                <option value="1">Active</option>
                                <option value="0">Inactive</option>
                            </select>
                        </div>
                        <div class="col-lg-2">
                            <button type="button" class="btn btn-primary w-100" id="btn-add">
                                <i class="ti ti-plus opacity-50 fs-2 me-1"></i>
                                Create New
                            </button>
                        </div>
                    </div>
                    <div class="card-body" id="table-default">
                       <table class="table table-hover table-vcenter  card-table w-100" id="data-table">
                            <thead>
                                <tr>
                                    <th>No</th>
                                    <th>#</th>
                                    <th>design type</th>
                                    <th>fabric name</th>
                                    <th>width</th>
                                    <th>group</th>
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
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalSubmitLabel">Modal title</h1>
                    <button type="button" class="btn-close force-close" aria-label="Close"></button>
                </div>
                <form action="javascript:void(0)" method="POST" id="form-submit">
                    <div class="modal-body">
                        <div class="row">
                            <div class="mb-3 col-lg-2">
                                <label for="id" class="form-label text-uppercase">fabric id</label>
                                <input type="number" name="id" id="id" class="form-control">
                                <input type="text" name="action" id="action" class="form-control" readonly hidden>
                            </div>
                            <div class="mb-3 col-lg-10">
                                <label for="name" class="form-label text-uppercase">fabric name</label>
                                <input type="text" name="name" id="name" class="form-control">
                            </div>
                            <div class="mb-3 col-lg-6">
                                <label for="type" class="form-label text-uppercase">fabric type</label>
                                <input type="text" name="type" id="type" class="form-control">
                            </div>
                            <div class="mb-3 col-lg-6">
                                <label for="colour" class="form-label text-uppercase">fabric colour</label>
                                <input type="text" name="colour" id="colour" class="form-control">
                            </div>
                            <div class="mb-3 col-lg-2">
                                <label for="width" class="form-label text-uppercase">fabric width</label>
                                <input type="number" name="width" id="width" class="form-control">
                            </div>
                            <div class="mb-3 col-lg-5">
                                <label for="group" class="form-label text-uppercase">fabric group</label>
                                <input type="text" name="group" id="group" class="form-control">
                            </div>
                            <div class="mb-3 col-lg-5">
                                <label for="designtype" class="form-label text-uppercase">design type</label>
                                <select name="designtype" id="designtype" class="form-select"></select>
                            </div>
                            <div class="mb-3 col-lg-10">
                                <label for="des" class="form-label text-uppercase">description</label>
                                <textarea name="des" id="des" class="form-control" cols="1" rows="1"></textarea>
                            </div>
                            <div class="mb-3 col-lg-2">
                                <label for="activate" class="form-label text-uppercase">active</label>
                                <select name="activate" id="activate" class="form-select">
                                    <option value="1">Active</option>
                                    <option value="0">Inactive</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary force-close">Close</button>
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
        let uriMethod = '/Methods/Setting/Fabric/FabricMethod.aspx'; 

        
        // close modal
        document.querySelectorAll(".modal .force-close").forEach(modalEl => {
            modalEl.addEventListener("click", (e) => {
                // close modal
                handlerHideBSModal(modalEl.closest('.modal').id);
            })
        })
    </script>
    <script src="/Scripts/Setting/Fabric/Default.js?<%= DateTime.Now.Ticks %>"></script>

</asp:Content>

