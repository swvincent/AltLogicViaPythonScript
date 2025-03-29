#! /usr/bin/env python3

"""
Custom Process

Arguments:
    1 - connection string
    2 - purchase order ID
"""

import pymongo
from pymongo import MongoClient
from bson.objectid import ObjectId
import sys

def customProcess(uri, objId):
    client = pymongo.MongoClient(uri)
    db = client["OrderSystem"]
    col = db["PurchaseOrders"]

    myFilter = { '_id' : ObjectId(objId)}

    # Apply 5% discount
    pipeline = [
       {
           "$set": {
               "AdjustedOrderTotal": {
                   "$multiply": [ 0.95, "$OrderTotal" ]
               }
           }
       },
    ]

    results = col.update_one(myFilter, pipeline)

    return results.acknowledged

if __name__ == '__main__':
    print(customProcess(sys.argv[1], sys.argv[2]))
