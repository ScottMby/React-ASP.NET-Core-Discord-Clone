namespace Discord_Clone.Server.Utilities
{
    public class IHttpException : Exception
    {
        public virtual int Status { get; } = StatusCodes.Status500InternalServerError;
        public virtual string Title { get; } = "Internal Server Error";
        public virtual string Type { get; } = "6.5.0";

        public IHttpException() : base() { }
        public IHttpException(string? message) : base(message) { }
        public IHttpException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class BadRequestException : IHttpException
    {
        public override int Status => StatusCodes.Status400BadRequest;
        public override string Title => "Bad Request";
        public override string Type => "6.5.1";

        public BadRequestException() : base() { }
        public BadRequestException(string? message) : base(message) { }
        public BadRequestException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class UnauthorizedException : IHttpException
    {
        public override int Status => StatusCodes.Status401Unauthorized;
        public override string Title => "Unauthorized";
        public override string Type => "6.5.2";

        public UnauthorizedException() : base() { }
        public UnauthorizedException(string? message) : base(message) { }
        public UnauthorizedException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class ForbiddenException : IHttpException
    {
        public override int Status => StatusCodes.Status403Forbidden;
        public override string Title => "Forbidden";
        public override string Type => "6.5.3";

        public ForbiddenException() : base() { }
        public ForbiddenException(string? message) : base(message) { }
        public ForbiddenException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class NotFoundException : IHttpException
    {
        public override int Status => StatusCodes.Status404NotFound;
        public override string Title => "Not Found";
        public override string Type => "6.5.4";

        public NotFoundException() : base() { }
        public NotFoundException(string? message) : base(message) { }
        public NotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class MethodNotAllowedException : IHttpException
    {
        public override int Status => StatusCodes.Status405MethodNotAllowed;
        public override string Title => "Method Not Allowed";
        public override string Type => "6.5.5";

        public MethodNotAllowedException() : base() { }
        public MethodNotAllowedException(string? message) : base(message) { }
        public MethodNotAllowedException(string? message, Exception? innerException) : base(message, innerException) { }
    }

    public class InternalServerErrorException : IHttpException
    {
        public override int Status => StatusCodes.Status500InternalServerError;
        public override string Title => "Internal Server Error";
        public override string Type => "6.6.1";

        public InternalServerErrorException() : base() { }
        public InternalServerErrorException(string? message) : base(message) { }
        public InternalServerErrorException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
