namespace Glass.Core.HTTP.Interfaces {
    
    interface IHTTPRouter {

        void Open();

        bool AcceptPost();
        bool AcceptGet();
        bool AcceptDelete();
        bool AcceptPut();

    }

}
