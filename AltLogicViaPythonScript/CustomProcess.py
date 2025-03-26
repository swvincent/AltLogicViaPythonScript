from pymongo import MongoClient

import pymongo
from bson.objectid import ObjectId

client = pymongo.MongoClient("mongodb://localhost:27018/")
db = client["OrderSystem"]
col = db["PurchaseOrders"]

myFilter = { '_id' : ObjectId('67e35154716e79e09821e062')}

pipeline = [
   {
      "$set": {
         "AdjustedOrderTotal": "$OrderTotal"
      }
   },
]

results = col.update_one(myFilter, pipeline)
