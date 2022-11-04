using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;

// To interact with Amazon S3.
using Amazon.S3;
using Amazon.S3.Model;

namespace S3CreateAndList
{
    class Program
    {
        private readonly static string _BUCKETNAME = "rstests3nonpublic";
        private readonly static string _ERRORPREFIX = "ERROR: ";
        private readonly static string _INFOPREFIX = "INFO: ";
        // Main method
        static async Task Main(string[] args)
        {
            await ListBucketsRequest();
            await ListBucketContentsAsync(_BUCKETNAME);

        }



        /// <summary>
        /// Method to  List Bucket names for a certain AWS user account.
        /// </summary>
        /// <param name="args"></param>
        private async static Task ListBucketsRequest()
        {
            // Before running this app:
            // - Credentials must be specified in an AWS profile. If you use a profile other than
            //   the [default] profile, also set the AWS_PROFILE environment variable.
            // - An AWS Region must be specified either in the [default] profile
            //   or by setting the AWS_REGION environment variable.
            // Create an S3 client object.
            var s3Client = new AmazonS3Client(RegionEndpoint.USEast2);


            // List the buckets owned by the user.
            // Call a class method that calls the API method.
            Console.WriteLine($" {_INFOPREFIX} Getting a list of your buckets...");
            var listResponse = await MyListBucketsAsync(s3Client);
            Console.WriteLine($" {_INFOPREFIX} Number of buckets: {listResponse.Buckets.Count}");
            foreach (S3Bucket b in listResponse.Buckets)
            {
                Console.WriteLine( $"\t     {b.BucketName}");
            }

        }

        /// <summary>
        /// Shows how to list the objects in an Amazon S3 bucket.
        /// </summary>
        /// <param name="client">An initialized Amazon S3 client object.</param>
        /// <param name="bucketName">The name of the bucket for which to list
        /// the contents.</param>
        /// <returns>A boolean value indicating the success or failure of the
        /// copy operation.</returns>
        public static async Task<bool> ListBucketContentsAsync(string bucketName)
        {
            // Create an S3 client object.
            var s3Client = new AmazonS3Client(RegionEndpoint.USEast1);


            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = bucketName
                            
                };

                Console.WriteLine($" {_INFOPREFIX} Listing the contents of {bucketName}:");

                var response = new ListObjectsV2Response();

                do
                {
                    response = await s3Client.ListObjectsV2Async(request);

                    response.S3Objects
                        .ForEach(obj => Console.WriteLine($"\t    {obj.Key,-35}{obj.LastModified.ToShortDateString(),10}{obj.Size,10}"));

                    // If the response is truncated, set the request ContinuationToken
                    // from the NextContinuationToken property of the response.
                    request.ContinuationToken = response.NextContinuationToken;
                }
                while (response.IsTruncated);

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($" {_ERRORPREFIX} encountered on server. Message:'{ex.Message}' getting list of objects.");
                return false;
            }
        }


        // 
        // Method to parse the command line.
        private static Boolean GetBucketName(string[] args, out String bucketName)
        {
            Boolean retval = false;
            bucketName = String.Empty;
            if (args.Length == 0)
            {
                Console.WriteLine("\nNo arguments specified. Will simply list your Amazon S3 buckets." +
                  "\nIf you wish to create a bucket, supply a valid, globally unique bucket name.");
                bucketName = String.Empty;
                retval = false;
            }
            else if (args.Length == 1)
            {
                bucketName = args[0];
                retval = true;
            }
            else
            {
                Console.WriteLine("\nToo many arguments specified." +
                  "\n\ndotnet_tutorials - A utility to list your Amazon S3 buckets and optionally create a new one." +
                  "\n\nUsage: S3CreateAndList [bucket_name]" +
                  "\n - bucket_name: A valid, globally unique bucket name." +
                  "\n - If bucket_name isn't supplied, this utility simply lists your buckets.");
                Environment.Exit(1);
            }
            return retval;
        }


        //
        // Async method to get a list of Amazon S3 buckets.
        private static async Task<ListBucketsResponse> MyListBucketsAsync(IAmazonS3 s3Client)
        {
            return await s3Client.ListBucketsAsync();
        }

    }
}
